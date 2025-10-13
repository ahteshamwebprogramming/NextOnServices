(function ($) {
    'use strict';

    if (!$ || !$.fn) {
        return;
    }

    const PANEL_SELECTOR = '#projectChatPanel';
    const POLL_INTERVAL = 30000;

    const state = {
        $panel: null,
        $sheet: null,
        $log: null,
        $empty: null,
        $input: null,
        $form: null,
        $status: null,
        $title: null,
        $subtitle: null,
        $meta: null,
        activeProject: null,
        pollTimer: null,
        isLoading: false
    };

    const messageFields = {
        text: ['message', 'text', 'body', 'content', 'note'],
        id: ['id', 'messageId', 'conversationMessageId', 'chatId'],
        timestamp: ['createdAt', 'createdOn', 'createdUtc', 'created', 'sentAt', 'timestamp', 'date', 'loggedAt'],
        sender: ['sender', 'from', 'author', 'owner', 'createdByName']
    };

    $(document).on('click', '.project-chat-trigger', function (event) {
        event.preventDefault();
        ensurePanel();

        if (!state.$panel) {
            return;
        }

        const $trigger = $(this);
        const project = buildProjectFromTrigger($trigger);

        if (!project.projectMappingId && !project.projectId) {
            console.warn('[chatPanel] Missing project identifiers for chat trigger.', project);
        }

        openPanel(project);
    });

    $(document).ready(function () {
        ensurePanel();

        if (!state.$panel) {
            return;
        }

        bindPanelEvents();
    });

    function ensurePanel() {
        if (state.$panel && state.$panel.length) {
            return;
        }

        const $panel = $(PANEL_SELECTOR);
        if (!$panel.length) {
            return;
        }

        state.$panel = $panel;
        state.$sheet = $panel.find('.project-chat__sheet');
        state.$log = $panel.find('[data-chat-log]');
        state.$empty = $panel.find('[data-chat-empty]');
        state.$input = $panel.find('[data-chat-input]');
        state.$form = $panel.find('[data-chat-form]');
        state.$status = $panel.find('[data-chat-status]');
        state.$title = $panel.find('[data-chat-title]');
        state.$subtitle = $panel.find('[data-chat-subtitle]');
        state.$meta = $panel.find('[data-chat-meta]');
    }

    function bindPanelEvents() {
        if (!state.$panel) {
            return;
        }

        state.$panel.on('click', '[data-action="close-chat"]', function (event) {
            event.preventDefault();
            closePanel();
        });

        state.$form.on('submit', function (event) {
            event.preventDefault();
            sendMessage();
        });

        state.$input.on('input', function () {
            updateSendAvailability();
        });

        state.$input.on('keydown', function (event) {
            if (event.key === 'Enter' && !event.shiftKey) {
                event.preventDefault();
                sendMessage();
            }
        });

        state.$panel.on('click', '.project-chat__backdrop', function () {
            closePanel();
        });

        $(document).on('keydown.chatPanel', function (event) {
            if (event.key === 'Escape' && state.$panel?.hasClass('is-open')) {
                closePanel();
            }
        });
    }

    function normalizeId(value) {
        if (value === undefined || value === null) {
            return null;
        }

        const stringValue = String(value).trim();
        if (!stringValue) {
            return null;
        }

        const numericValue = Number(stringValue);
        if (!Number.isFinite(numericValue) || !Number.isInteger(numericValue)) {
            return null;
        }

        return numericValue;
    }

    function buildProjectFromTrigger($trigger) {
        const decode = (value) => $('<textarea />').html(value || '').text();
        const project = {
            projectMappingId: normalizeId($trigger.data('project-id')),
            projectId: normalizeId($trigger.data('projectId')),
            supplierId: normalizeId($trigger.data('supplierId')),
            pid: decode($trigger.data('pid')) || '',
            projectName: decode($trigger.data('project-name')) || '',
            unreadCount: Number($trigger.data('unread-count') ?? 0) || 0,
            lastMessage: $trigger.data('last-message') || '',
            historyUrl: $trigger.data('history-url') || '',
            sendUrl: $trigger.data('send-url') || '',
            pollUrl: $trigger.data('poll-url') || '',
            $trigger: $trigger
        };

        if (!project.historyUrl) {
            project.historyUrl = state.$panel.data('history-url') || '';
        }

        if (!project.sendUrl) {
            project.sendUrl = state.$panel.data('send-url') || '';
        }

        if (!project.pollUrl) {
            project.pollUrl = state.$panel.data('poll-url') || '';
        }

        return project;
    }

    function openPanel(project) {
        if (!state.$panel) {
            return;
        }

        state.activeProject = project;
        state.$panel.addClass('is-open').attr('aria-hidden', 'false');
        $('body').addClass('project-chat-open');

        updateHeader(project);
        clearLog();
        markTriggerAsRead(project.$trigger);
        fetchHistory();
        startPolling();

        setTimeout(function () {
            state.$input.trigger('focus');
        }, 200);
    }

    function closePanel() {
        if (!state.$panel) {
            return;
        }

        state.$panel.removeClass('is-open').attr('aria-hidden', 'true');
        $('body').removeClass('project-chat-open');
        stopPolling();
        state.activeProject = null;
        resetStatus();
    }

    function updateHeader(project) {
        if (!state.$title || !state.$subtitle) {
            return;
        }

        const title = project.projectName || project.pid || 'Project chat';
        state.$title.text(title);

        const subtitleParts = [];
        if (project.pid) {
            subtitleParts.push(`Project #${project.pid}`);
        }
        if (project.projectMappingId) {
            subtitleParts.push(`Mapping ID ${project.projectMappingId}`);
        }
        state.$subtitle.text(subtitleParts.join(' · '));

        if (state.$meta) {
            const lastMessage = project.lastMessage ? formatTimestamp(project.lastMessage) : '';
            state.$meta.toggle(!!lastMessage).text(lastMessage ? `Last message ${lastMessage}` : '');
        }
    }

    function clearLog() {
        if (!state.$log) {
            return;
        }

        state.$log.empty();
        state.$empty.show();
    }

    function fetchHistory() {
        const project = state.activeProject;
        if (!project || !project.historyUrl) {
            state.$log && state.$log.empty();
            toggleLoading(false);
            showStatus('Chat history is unavailable.', 'warning');
            return;
        }

        toggleLoading(true);
        $.ajax({
            url: project.historyUrl,
            method: 'GET',
            dataType: 'json',
            data: buildHistoryRequest(project)
        }).done(function (response) {
            const messages = normaliseMessages(response);
            renderMessages(messages, project);
            if (messages.length) {
                const lastMessage = messages[messages.length - 1];
                project.lastMessage = lastMessage.timestamp || lastMessage.createdAt || lastMessage.createdOn || project.lastMessage;
            }
            resetStatus();
        }).fail(function (xhr) {
            console.error('[chatPanel] Failed to fetch chat history', xhr);
            showStatus('Unable to load chat history. Please try again later.', 'error');
        }).always(function () {
            toggleLoading(false);
        });
    }

    function buildHistoryRequest(project) {
        return {
            projectMappingId: project.projectMappingId,
            projectId: project.projectId,
            supplierId: project.supplierId,
            pid: project.pid
        };
    }

    function renderMessages(messages, project) {
        if (!state.$log) {
            return;
        }

        state.$log.empty();

        if (!messages.length) {
            state.$empty.show();
            return;
        }

        state.$empty.hide();

        messages.forEach(function (message) {
            const meta = shapeMessage(message, project);
            appendMessage(meta, { scroll: false });
        });

        scrollToBottom();
    }

    function appendMessage(message, options) {
        if (!state.$log) {
            return;
        }

        const settings = $.extend({ scroll: true }, options);
        const classes = ['chat-message'];
        if (message.isMine) {
            classes.push('chat-message--outgoing');
        } else {
            classes.push('chat-message--incoming');
        }

        if (message.optimistic) {
            classes.push('chat-message--optimistic');
        }

        if (message.error) {
            classes.push('chat-message--error');
        }

        const $message = $('<div/>', {
            class: classes.join(' '),
            'data-message-id': message.id || '',
            'data-temp-id': message.tempId || ''
        });

        const $bubble = $('<div/>', { class: 'chat-message__bubble' }).text(message.text || '');
        $message.append($bubble);

        if (message.timestamp) {
            $('<div/>', { class: 'chat-message__meta', text: formatTimestamp(message.timestamp) }).appendTo($message);
        }

        if (message.error) {
            $('<div/>', { class: 'chat-message__error text-danger', text: message.error }).appendTo($message);
        }

        state.$log.append($message);

        if (settings.scroll) {
            scrollToBottom();
        }
    }

    function replaceOptimisticMessage(tempId, message) {
        if (!state.$log) {
            return;
        }

        const $existing = state.$log.find(`[data-temp-id="${tempId}"]`).first();
        if (!$existing.length) {
            appendMessage(message, { scroll: true });
            return;
        }

        const classes = ['chat-message'];
        classes.push(message.isMine ? 'chat-message--outgoing' : 'chat-message--incoming');

        $existing.attr('data-message-id', message.id || '');
        $existing.removeClass('chat-message--optimistic chat-message--error').attr('class', classes.join(' '));
        $existing.find('.chat-message__bubble').text(message.text || '');

        const formattedTimestamp = message.timestamp ? formatTimestamp(message.timestamp) : '';
        if (formattedTimestamp) {
            let $meta = $existing.find('.chat-message__meta');
            if (!$meta.length) {
                $meta = $('<div/>', { class: 'chat-message__meta' }).appendTo($existing);
            }
            $meta.text(formattedTimestamp);
        }

        $existing.find('.chat-message__error').remove();
    }

    function sendMessage() {
        const project = state.activeProject;
        if (!project || !state.$input) {
            return;
        }

        const value = state.$input.val();
        const trimmed = (value || '').trim();
        if (!trimmed) {
            return;
        }

        const sendUrl = project.sendUrl;
        if (!sendUrl) {
            showStatus('Sending is disabled. Please contact support.', 'warning');
            return;
        }

        const tempId = `tmp-${Date.now()}`;
        const optimisticMessage = {
            id: '',
            tempId: tempId,
            text: trimmed,
            timestamp: new Date().toISOString(),
            isMine: true,
            optimistic: true
        };

        appendMessage(optimisticMessage, { scroll: true });
        state.$input.val('');
        updateSendAvailability();

        $.ajax({
            url: sendUrl,
            method: 'POST',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(buildSendPayload(project, trimmed))
        }).done(function (response) {
            const message = shapeMessage(response, project);
            message.tempId = tempId;
            replaceOptimisticMessage(tempId, message);
            project.lastMessage = message.timestamp || new Date().toISOString();
            resetStatus();
        }).fail(function (xhr) {
            console.error('[chatPanel] Failed to send message', xhr);
            markOptimisticAsFailed(tempId, xhr);
        });
    }

    function buildSendPayload(project, message) {
        const payload = {
            pid: project.pid || null,
            message: message
        };

        const identifiers = {
            projectMappingId: normalizeId(project.projectMappingId),
            projectId: normalizeId(project.projectId),
            supplierId: normalizeId(project.supplierId)
        };

        Object.keys(identifiers).forEach(function (key) {
            payload[key] = identifiers[key];
        });

        return payload;
    }

    function markOptimisticAsFailed(tempId, xhr) {
        if (!state.$log) {
            return;
        }

        const $existing = state.$log.find(`[data-temp-id="${tempId}"]`).first();
        if (!$existing.length) {
            return;
        }

        $existing.addClass('chat-message--error');
        const errorText = deriveErrorMessage(xhr);
        let $error = $existing.find('.chat-message__error');
        if (!$error.length) {
            $error = $('<div/>', { class: 'chat-message__error text-danger' }).appendTo($existing);
        }
        $error.text(errorText);
        showStatus(errorText, 'error');
    }

    function deriveErrorMessage(xhr) {
        if (!xhr) {
            return 'Unable to send your message. Please retry.';
        }

        if (xhr.responseJSON?.message) {
            return xhr.responseJSON.message;
        }

        if (xhr.responseText) {
            try {
                const parsed = JSON.parse(xhr.responseText);
                if (parsed?.message) {
                    return parsed.message;
                }
            } catch (err) {
                return 'Unable to send your message. Please retry.';
            }
        }

        return 'Unable to send your message. Please retry.';
    }

    function updateSendAvailability() {
        if (!state.$form) {
            return;
        }

        const $button = state.$form.find('[data-action="send-chat"]');
        if (!$button.length) {
            return;
        }

        const hasText = !!(state.$input?.val() || '').trim();
        $button.prop('disabled', !hasText);
    }

    function startPolling() {
        stopPolling();

        if (!state.activeProject?.pollUrl) {
            return;
        }

        state.pollTimer = window.setInterval(function () {
            pollForUpdates();
        }, POLL_INTERVAL);
    }

    function stopPolling() {
        if (state.pollTimer) {
            window.clearInterval(state.pollTimer);
            state.pollTimer = null;
        }
    }

    function pollForUpdates() {
        const project = state.activeProject;
        if (!project || !project.pollUrl) {
            return;
        }

        $.ajax({
            url: project.pollUrl,
            method: 'GET',
            dataType: 'json',
            data: {
                projectMappingId: project.projectMappingId,
                after: project.lastMessage
            }
        }).done(function (response) {
            const messages = normaliseMessages(response);
            if (!messages.length) {
                return;
            }

            messages.forEach(function (item) {
                const shaped = shapeMessage(item, project);
                appendMessage(shaped, { scroll: true });
                project.lastMessage = shaped.timestamp || project.lastMessage;
            });

            resetStatus();
        }).fail(function (xhr) {
            console.warn('[chatPanel] Polling failed', xhr);
        });
    }

    function normaliseMessages(payload) {
        if (!payload) {
            return [];
        }

        if (Array.isArray(payload)) {
            return payload;
        }

        if ($.isPlainObject(payload)) {
            if (Array.isArray(payload.messages)) {
                return payload.messages;
            }

            if (Array.isArray(payload.data)) {
                return payload.data;
            }

            if (Array.isArray(payload.items)) {
                return payload.items;
            }

            if (payload.results && Array.isArray(payload.results)) {
                return payload.results;
            }
        }

        return [];
    }

    function shapeMessage(message, project) {
        if ($.isArray(message) && message.length >= 2) {
            message = { message: message[1], createdAt: message[0] };
        }

        const text = extractField(message, messageFields.text) || '';
        const timestamp = extractField(message, messageFields.timestamp) || new Date().toISOString();
        const sender = extractField(message, messageFields.sender) || '';
        const id = extractField(message, messageFields.id) || message?.tempId || '';

        const isMine = determineIsMine(message, project, sender);

        return {
            id: id,
            text: text,
            timestamp: timestamp,
            sender: sender,
            isMine: isMine,
            optimistic: message.optimistic || false
        };
    }

    function extractField(source, candidates) {
        if (!source) {
            return '';
        }

        for (let i = 0; i < candidates.length; i += 1) {
            const key = candidates[i];
            if (source[key] !== undefined && source[key] !== null) {
                return source[key];
            }
        }

        return '';
    }

    function determineIsMine(message, project, sender) {
        if (typeof message.isMine === 'boolean') {
            return message.isMine;
        }

        if (typeof message.isOutbound === 'boolean') {
            return message.isOutbound;
        }

        if (typeof message.fromSupplier === 'boolean') {
            return message.fromSupplier;
        }

        if (message.direction) {
            const direction = String(message.direction).toLowerCase();
            if (direction === 'out' || direction === 'outbound' || direction === 'sent') {
                return true;
            }
            if (direction === 'in' || direction === 'inbound' || direction === 'received') {
                return false;
            }
        }

        if (message.senderId && project?.supplierId) {
            return String(message.senderId) === String(project.supplierId);
        }

        if (sender && project?.projectName) {
            return sender.toString().toLowerCase().indexOf(project.projectName.toLowerCase()) >= 0;
        }

        return false;
    }

    function markTriggerAsRead($trigger) {
        if (!$trigger || !$trigger.length) {
            return;
        }

        $trigger.attr('data-unread-count', 0);
        $trigger.find('[data-unread-badge]').remove();
    }

    function toggleLoading(isLoading) {
        state.isLoading = !!isLoading;
        state.$panel && state.$panel.toggleClass('is-loading', state.isLoading);
    }

    function showStatus(message, tone) {
        if (!state.$status) {
            return;
        }

        state.$status.removeClass('text-danger text-warning text-success');

        if (!message) {
            state.$status.text('');
            return;
        }

        const classMap = {
            error: 'text-danger',
            warning: 'text-warning',
            success: 'text-success'
        };

        const className = classMap[tone];
        if (className) {
            state.$status.addClass(className);
        }

        state.$status.text(message);
    }

    function resetStatus() {
        showStatus('', '');
    }

    function scrollToBottom() {
        if (!state.$log) {
            return;
        }

        state.$log.scrollTop(state.$log.prop('scrollHeight'));
    }

    function formatTimestamp(value) {
        if (!value) {
            return '';
        }

        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }

        return date.toLocaleString(undefined, {
            hour: '2-digit',
            minute: '2-digit',
            year: 'numeric',
            month: 'short',
            day: '2-digit'
        });
    }
})(window.jQuery);
