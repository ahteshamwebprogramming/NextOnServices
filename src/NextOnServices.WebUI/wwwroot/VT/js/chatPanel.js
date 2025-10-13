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
        isLoading: false,
        userContext: {}
    };

    const messageFields = {
        text: ['message', 'Message', 'text', 'Text', 'body', 'Body', 'content', 'Content', 'note', 'Note'],
        id: ['id', 'Id', 'messageId', 'MessageId', 'conversationMessageId', 'chatId'],
        timestamp: ['createdAt', 'CreatedAt', 'createdOn', 'CreatedOn', 'createdUtc', 'CreatedUtc', 'created', 'Created', 'sentAt', 'SentAt', 'timestamp', 'Timestamp', 'date', 'Date', 'loggedAt', 'LoggedAt'],
        sender: ['sender', 'Sender', 'from', 'From', 'author', 'Author', 'owner', 'Owner', 'createdByName', 'CreatedByName', 'createdByDisplayName', 'CreatedByDisplayName']
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
        state.userContext = buildUserContext($panel);
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

    function normaliseBoolean(value) {
        if (value === undefined || value === null) {
            return null;
        }

        if (typeof value === 'boolean') {
            return value;
        }

        if (typeof value === 'number') {
            if (value === 1) {
                return true;
            }

            if (value === 0) {
                return false;
            }
        }

        if (typeof value === 'string') {
            const trimmed = value.trim().toLowerCase();
            if (!trimmed) {
                return null;
            }

            if (['true', '1', 'yes', 'y', 'on', 'supplier', 'vendor'].indexOf(trimmed) >= 0) {
                return true;
            }

            if (['false', '0', 'no', 'n', 'off', 'admin', 'staff', 'user', 'internal'].indexOf(trimmed) >= 0) {
                return false;
            }
        }

        return null;
    }

    function buildUserContext($panel) {
        const context = {
            role: '',
            supplierId: null,
            isSupplierUser: null,
            name: ''
        };

        if (!$panel || !$panel.length) {
            return context;
        }

        const role = $panel.data('currentUserRole');
        if (role !== undefined) {
            context.role = role || '';
        }

        const supplierId = normalizeId($panel.data('currentUserSupplierId'));
        if (supplierId !== null) {
            context.supplierId = supplierId;
        }

        const isSupplier = normaliseBoolean($panel.data('isSupplierUser'));
        if (isSupplier !== null) {
            context.isSupplierUser = isSupplier;
        }

        const name = $panel.data('currentUserName');
        if (name !== undefined) {
            context.name = name || '';
        }

        if (context.isSupplierUser === null) {
            context.isSupplierUser = false;
        }

        return context;
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

        const $placeholder = state.$empty;

        if ($placeholder && $placeholder.length) {
            if (!$placeholder.parent().is(state.$log)) {
                state.$log.prepend($placeholder);
            }

            state.$log.children().not($placeholder).remove();
            updateEmptyPlaceholder($placeholder);
            $placeholder.show();
            return;
        }

        state.$log.empty();
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

            const nextCursor = extractNextCursor(response);
            let fallbackCursor = null;

            if (!nextCursor && messages.length) {
                const lastMessage = messages[messages.length - 1];
                const shaped = shapeMessage(lastMessage, project);
                fallbackCursor = shaped.timestamp;
            }

            setProjectCursor(project, nextCursor || fallbackCursor);
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

        const $placeholder = state.$empty;

        if ($placeholder && $placeholder.length) {
            if (!$placeholder.parent().is(state.$log)) {
                state.$log.prepend($placeholder);
            }

            state.$log.children().not($placeholder).remove();
            updateEmptyPlaceholder($placeholder);
        } else {
            state.$log.empty();
        }

        if (!messages.length) {
            if ($placeholder && $placeholder.length) {
                $placeholder.show();
            }
            return;
        }

        if ($placeholder && $placeholder.length) {
            $placeholder.hide();
        }

        messages.forEach(function (message) {
            const meta = shapeMessage(message, project);
            appendMessage(meta, { scroll: false });
        });

        scrollToBottom();
    }

    function updateEmptyPlaceholder($placeholder) {
        if (!$placeholder || !$placeholder.length) {
            return;
        }

        const $placeholderText = $placeholder.find('p').first();
        if ($placeholderText.length) {
            $placeholderText.text('No chats to display');
        } else {
            $placeholder.text('No chats to display');
        }
    }

    function appendMessage(message, options) {
        if (!state.$log) {
            return;
        }

        if (state.$empty) {
            state.$empty.hide();
        }

        const settings = $.extend({ scroll: true }, options);
        const $existing = findExistingMessageElement(message);

        if ($existing.length) {
            refreshMessageElement($existing, message);
            if (settings.scroll) {
                scrollToBottom();
            }
            return;
        }

        const $message = $('<div/>', { class: 'chat-message' });
        $('<div/>', { class: 'chat-message__bubble' }).appendTo($message);

        refreshMessageElement($message, message);

        state.$log.append($message);

        if (settings.scroll) {
            scrollToBottom();
        }
    }

    function replaceOptimisticMessage(tempId, message) {
        if (!state.$log) {
            return;
        }

        const hydrated = $.extend({}, message, { tempId: tempId });
        const $existing = findMessageByAttribute('data-temp-id', tempId);
        if (!$existing.length) {
            appendMessage(hydrated, { scroll: true });
            return;
        }

        refreshMessageElement($existing, hydrated);
    }

    function normaliseIdentityValue(value) {
        if (value === undefined || value === null) {
            return '';
        }

        return String(value).trim();
    }

    function findMessageByAttribute(attribute, value) {
        if (!state.$log) {
            return $();
        }

        const normalisedValue = normaliseIdentityValue(value);
        if (!attribute || !normalisedValue) {
            return $();
        }

        const $elements = state.$log.children().filter(function () {
            const existing = normaliseIdentityValue($(this).attr(attribute));
            return existing && existing === normalisedValue;
        });

        return $elements.first();
    }

    function findExistingMessageElement(message) {
        if (!message) {
            return $();
        }

        let $existing = findMessageByAttribute('data-temp-id', message.tempId);
        if ($existing.length) {
            return $existing;
        }

        $existing = findMessageByAttribute('data-message-id', message.id);
        if ($existing.length) {
            return $existing;
        }

        return findMessageByAttribute('data-message-timestamp', message.timestamp);
    }

    function refreshMessageElement($element, message) {
        if (!$element || !$element.length) {
            return;
        }

        const classes = ['chat-message'];
        classes.push(message.isMine ? 'chat-message--outgoing' : 'chat-message--incoming');

        if (message.optimistic) {
            classes.push('chat-message--optimistic');
        }

        if (message.error) {
            classes.push('chat-message--error');
        }

        $element.attr('class', classes.join(' '));
        $element.attr('data-message-id', normaliseIdentityValue(message.id));
        $element.attr('data-temp-id', normaliseIdentityValue(message.tempId));
        $element.attr('data-message-timestamp', normaliseIdentityValue(message.timestamp));

        let $bubble = $element.find('.chat-message__bubble');
        if (!$bubble.length) {
            $bubble = $('<div/>', { class: 'chat-message__bubble' }).prependTo($element);
        }
        $bubble.text(message.text || '');

        updateMessageMeta($element, message);

        const $error = $element.find('.chat-message__error');
        if (message.error) {
            if ($error.length) {
                $error.text(message.error);
            } else {
                $('<div/>', { class: 'chat-message__error text-danger', text: message.error }).appendTo($element);
            }
        } else if ($error.length) {
            $error.remove();
        }
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
        const userContext = state.userContext || {};
        const optimisticMessage = {
            id: '',
            tempId: tempId,
            text: trimmed,
            timestamp: new Date().toISOString(),
            isMine: true,
            sender: userContext.name || 'You',
            fromSupplier: typeof userContext.isSupplierUser === 'boolean' ? userContext.isSupplierUser : null,
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

        const requestData = {
            projectMappingId: project.projectMappingId
        };

        if (project.lastMessage) {
            requestData.after = project.lastMessage;
        }

        $.ajax({
            url: project.pollUrl,
            method: 'GET',
            dataType: 'json',
            data: requestData
        }).done(function (response) {
            const messages = normaliseMessages(response);
            if (!messages.length) {
                const cursor = extractNextCursor(response);
                if (cursor) {
                    setProjectCursor(project, cursor);
                }
                return;
            }

            const nextCursor = extractNextCursor(response);
            let lastTimestamp = null;

            messages.forEach(function (item) {
                const shaped = shapeMessage(item, project);
                appendMessage(shaped, { scroll: true });
                if (shaped.timestamp) {
                    lastTimestamp = shaped.timestamp;
                }
            });

            if (nextCursor) {
                setProjectCursor(project, nextCursor);
            } else if (lastTimestamp) {
                setProjectCursor(project, lastTimestamp);
            }

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

    function extractNextCursor(payload) {
        if (!payload || typeof payload !== 'object') {
            return null;
        }

        if (payload.nextCursor !== undefined && payload.nextCursor !== null) {
            return payload.nextCursor;
        }

        if (payload.NextCursor !== undefined && payload.NextCursor !== null) {
            return payload.NextCursor;
        }

        return null;
    }

    function setProjectCursor(project, cursor) {
        if (!project || cursor === undefined || cursor === null) {
            return;
        }

        let isoString = null;

        if (cursor instanceof Date && !Number.isNaN(cursor.getTime())) {
            isoString = cursor.toISOString();
        } else if (typeof cursor === 'number' && Number.isFinite(cursor)) {
            const fromNumber = new Date(cursor);
            if (!Number.isNaN(fromNumber.getTime())) {
                isoString = fromNumber.toISOString();
            }
        } else if (typeof cursor === 'string') {
            const trimmed = cursor.trim();
            if (trimmed) {
                const fromString = new Date(trimmed);
                if (!Number.isNaN(fromString.getTime())) {
                    isoString = fromString.toISOString();
                }
            }
        }

        if (isoString) {
            project.lastMessage = isoString;
        }
    }

    function updateMessageMeta($element, message) {
        if (!$element || !$element.length) {
            return;
        }

        const metaText = buildMetaText(message);
        let $meta = $element.find('.chat-message__meta');

        if (metaText) {
            if (!$meta.length) {
                $meta = $('<div/>', { class: 'chat-message__meta' }).appendTo($element);
            }

            $meta.text(metaText);
        } else if ($meta.length) {
            $meta.remove();
        }
    }

    function buildMetaText(message) {
        if (!message) {
            return '';
        }

        const parts = [];

        if (message.sender) {
            parts.push(message.sender);
        }

        const timestamp = message.timestamp ? formatTimestamp(message.timestamp) : '';
        if (timestamp) {
            parts.push(timestamp);
        }

        return parts.join(' · ');
    }

    function shapeMessage(message, project) {
        if ($.isArray(message) && message.length >= 2) {
            message = { message: message[1], createdAt: message[0] };
        }

        const text = extractField(message, messageFields.text) || '';
        const timestamp = extractField(message, messageFields.timestamp) || new Date().toISOString();
        const sender = extractField(message, messageFields.sender) || '';
        const id = extractField(message, messageFields.id) || message?.tempId || '';

        const fromSupplier = determineFromSupplier(message);
        const isMine = determineIsMine(message, project, sender, fromSupplier);

        return {
            id: id,
            text: text,
            timestamp: timestamp,
            sender: sender,
            isMine: isMine,
            fromSupplier: typeof fromSupplier === 'boolean' ? fromSupplier : null,
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

    function determineFromSupplier(message, explicit) {
        if (typeof explicit === 'boolean') {
            return explicit;
        }

        if (!message) {
            return null;
        }

        const candidates = [
            message.fromSupplier,
            message.FromSupplier,
            message.origin,
            message.senderType
        ];

        for (let i = 0; i < candidates.length; i += 1) {
            const candidate = candidates[i];
            const normalised = normaliseBoolean(candidate);
            if (normalised !== null) {
                return normalised;
            }
        }

        return null;
    }

    function determineIsMine(message, project, sender, fromSupplierFlag) {
        if (typeof message.isMine === 'boolean') {
            return message.isMine;
        }

        if (typeof message.isOutbound === 'boolean') {
            return message.isOutbound;
        }

        const userContext = state.userContext || {};
        const isSupplierUser = typeof userContext.isSupplierUser === 'boolean'
            ? userContext.isSupplierUser
            : null;

        const fromSupplier = determineFromSupplier(message, fromSupplierFlag);
        if (typeof fromSupplier === 'boolean' && isSupplierUser !== null) {
            return isSupplierUser ? fromSupplier : !fromSupplier;
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

        if (message.senderId && userContext.supplierId !== null && userContext.supplierId !== undefined) {
            return String(message.senderId) === String(userContext.supplierId);
        }

        if (message.senderId && project?.supplierId) {
            return String(message.senderId) === String(project.supplierId);
        }

        if (typeof fromSupplier === 'boolean') {
            return fromSupplier;
        }

        if (sender && project?.projectName) {
            return sender.toString().toLowerCase().indexOf(project.projectName.toLowerCase()) >= 0;
        }

        if (isSupplierUser !== null) {
            return isSupplierUser;
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

        const logElement = state.$log.get(0);
        if (!logElement) {
            return;
        }

        const maxScrollTop = logElement.scrollHeight - logElement.clientHeight;
        state.$log.scrollTop(Math.max(0, maxScrollTop));
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
