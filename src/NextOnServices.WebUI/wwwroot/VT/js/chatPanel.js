(function ($) {
    'use strict';

    if (!$ || !$.fn) {
        return;
    }

    const PANEL_SELECTOR = '#projectChatPanel';
    const POLL_INTERVAL = 30000;
    const ATTACHMENT_PREFIX = 'ATTACHMENT::';

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
        $attachmentInput: null,
        $attachmentPreview: null,
        $attachmentName: null,
        activeProject: null,
        pollTimer: null,
        isLoading: false,
        pendingAttachment: null
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
        state.$attachmentInput = $panel.find('[data-chat-attachment-input]');
        state.$attachmentPreview = $panel.find('[data-chat-attachment-preview]');
        state.$attachmentName = $panel.find('[data-chat-attachment-name]');
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

        state.$panel.on('click', '[data-action="choose-attachment"]', function (event) {
            event.preventDefault();
            openAttachmentPicker();
        });

        state.$panel.on('click', '[data-action="remove-attachment"]', function (event) {
            event.preventDefault();
            clearAttachment();
        });

        state.$panel.on('change', '[data-chat-attachment-input]', function () {
            const files = this.files || [];
            const file = files.length ? files[0] : null;
            setAttachment(file);
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
            attachmentUrl: $trigger.data('attachment-url') || '',
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

        if (!project.attachmentUrl) {
            project.attachmentUrl = state.$panel.data('attachment-url') || '';
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
        resetComposer();
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
        resetComposer();
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

        state.$log.children().each(function () {
            const previewUrl = $(this).data('previewUrl');
            if (previewUrl && typeof URL !== 'undefined' && typeof URL.revokeObjectURL === 'function') {
                URL.revokeObjectURL(previewUrl);
            }
            $(this).removeData('previewUrl');
        });

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
        const $message = $('<div/>', {
            class: 'chat-message',
            'data-message-id': message.id || '',
            'data-temp-id': message.tempId || ''
        });

        applyMessageClasses($message, message);
        renderMessageContent($message, message);
        updatePreviewData($message, message.attachment);

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

        $existing.attr('data-message-id', message.id || '');
        $existing.attr('data-temp-id', message.tempId || '');
        applyMessageClasses($existing, message);
        renderMessageContent($existing, message);
        updatePreviewData($existing, message.attachment);
    }

    function applyMessageClasses($message, message) {
        const classes = ['chat-message'];
        classes.push(message.isMine ? 'chat-message--outgoing' : 'chat-message--incoming');

        if (message.optimistic) {
            classes.push('chat-message--optimistic');
        }

        if (message.error) {
            classes.push('chat-message--error');
        }

        $message.attr('class', classes.join(' '));
    }

    function renderMessageContent($message, message) {
        if (!$message) {
            return;
        }

        $message.empty();

        if (!message.isMine && message.sender) {
            $('<div/>', { class: 'chat-message__sender', text: message.sender }).appendTo($message);
        }

        const $bubble = $('<div/>', { class: 'chat-message__bubble' }).appendTo($message);

        if (message.text) {
            $('<div/>', { class: 'chat-message__text' }).text(message.text).appendTo($bubble);
        }

        if (message.attachment) {
            renderAttachmentContent($bubble, message.attachment);
        }

        if (!message.text && !message.attachment) {
            $('<div/>', { class: 'chat-message__text' }).text('').appendTo($bubble);
        }

        if (message.timestamp) {
            $('<div/>', { class: 'chat-message__meta', text: formatTimestamp(message.timestamp) }).appendTo($message);
        }

        if (message.error) {
            $('<div/>', { class: 'chat-message__error text-danger', text: message.error }).appendTo($message);
        }
    }

    function renderAttachmentContent($container, attachment) {
        if (!$container || !attachment) {
            return;
        }

        const $attachment = $('<div/>', { class: 'chat-message__attachment' }).appendTo($container);
        const previewSrc = attachment.previewUrl || attachment.fileUrl || '';

        if (attachment.isImage && previewSrc) {
            $('<img/>', {
                class: 'chat-message__attachment-thumb',
                src: previewSrc,
                alt: attachment.caption || attachment.fileName || 'Attachment preview'
            }).appendTo($attachment);
        }

        const linkText = attachment.caption || attachment.fileName || 'View attachment';
        if (attachment.fileUrl) {
            $('<a/>', {
                class: 'chat-message__attachment-link',
                href: attachment.fileUrl,
                target: '_blank',
                rel: 'noopener',
                text: linkText
            }).appendTo($attachment);
        } else {
            $('<span/>', {
                class: 'chat-message__attachment-link chat-message__attachment-link--pending',
                text: linkText
            }).appendTo($attachment);
        }

        const metaParts = [];
        if (attachment.contentType) {
            metaParts.push(attachment.contentType);
        }

        const size = Number(attachment.length);
        if (Number.isFinite(size) && size > 0) {
            metaParts.push(formatBytes(size));
        }

        if (metaParts.length) {
            $('<div/>', { class: 'chat-message__attachment-meta', text: metaParts.join(' · ') }).appendTo($attachment);
        }
    }

    function updatePreviewData($message, attachment) {
        if (!$message) {
            return;
        }

        const existingPreview = $message.data('previewUrl');
        const nextPreview = attachment?.previewUrl;

        if (existingPreview && existingPreview !== nextPreview) {
            if (typeof URL !== 'undefined' && typeof URL.revokeObjectURL === 'function') {
                URL.revokeObjectURL(existingPreview);
            }
            $message.removeData('previewUrl');
        }

        if (nextPreview) {
            $message.data('previewUrl', nextPreview);
        }
    }

    function sendMessage() {
        const project = state.activeProject;
        if (!project || !state.$input) {
            return;
        }

        const value = state.$input.val();
        const trimmed = (value || '').trim();
        const file = state.pendingAttachment;

        if (!trimmed && !file) {
            return;
        }

        if (file) {
            sendAttachment(trimmed);
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
        resetComposer();

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

    function sendAttachment(caption) {
        const project = state.activeProject;
        if (!project) {
            return;
        }

        const file = state.pendingAttachment;
        if (!file) {
            return;
        }

        const uploadUrl = project.attachmentUrl || project.sendUrl;
        if (!uploadUrl) {
            showStatus('File uploads are disabled. Please contact support.', 'warning');
            return;
        }

        const tempId = `tmp-${Date.now()}`;
        const attachmentMeta = buildAttachmentMetadataFromFile(file, caption);
        const optimisticMessage = {
            id: '',
            tempId: tempId,
            text: caption || attachmentMeta.caption || attachmentMeta.fileName || '',
            timestamp: new Date().toISOString(),
            isMine: true,
            optimistic: true,
            attachment: attachmentMeta
        };

        appendMessage(optimisticMessage, { scroll: true });

        const formData = buildAttachmentPayload(project, caption, file);
        resetComposer();

        $.ajax({
            url: uploadUrl,
            method: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            dataType: 'json'
        }).done(function (response) {
            const message = shapeMessage(response, project);
            message.tempId = tempId;
            replaceOptimisticMessage(tempId, message);
            project.lastMessage = message.timestamp || new Date().toISOString();
            resetStatus();
        }).fail(function (xhr) {
            console.error('[chatPanel] Failed to upload attachment', xhr);
            markOptimisticAsFailed(tempId, xhr);
        });
    }

    function buildAttachmentPayload(project, caption, file) {
        const formData = new FormData();
        formData.append('file', file);

        const trimmedCaption = (caption || '').trim();
        formData.append('message', trimmedCaption);
        if (trimmedCaption) {
            formData.append('caption', trimmedCaption);
        }

        const identifiers = {
            projectMappingId: normalizeId(project.projectMappingId),
            projectId: normalizeId(project.projectId),
            supplierId: normalizeId(project.supplierId)
        };

        Object.keys(identifiers).forEach(function (key) {
            const value = identifiers[key];
            if (value !== null && value !== undefined) {
                formData.append(key, value);
            }
        });

        if (project.pid) {
            formData.append('pid', project.pid);
        }

        return formData;
    }

    function buildAttachmentMetadataFromFile(file, caption) {
        const meta = {
            fileName: file?.name || 'Attachment',
            caption: (caption || '').trim() || file?.name || 'Attachment',
            contentType: file?.type || '',
            length: file?.size || 0,
            isImage: false,
            fileUrl: '',
            previewUrl: ''
        };

        if (meta.contentType && meta.contentType.toLowerCase().indexOf('image/') === 0) {
            meta.isImage = true;
        }

        if (meta.isImage && typeof URL !== 'undefined' && typeof URL.createObjectURL === 'function') {
            meta.previewUrl = URL.createObjectURL(file);
        }

        return meta;
    }

    function openAttachmentPicker() {
        if (!state.$attachmentInput || !state.$attachmentInput.length) {
            return;
        }

        state.$attachmentInput.trigger('click');
    }

    function setAttachment(file) {
        if (!file) {
            clearAttachment();
            return;
        }

        state.pendingAttachment = file;

        if (state.$attachmentName) {
            state.$attachmentName.text(file.name);
        }

        if (state.$attachmentPreview) {
            state.$attachmentPreview.removeAttr('hidden');
        }

        if (state.$attachmentInput && state.$attachmentInput.length) {
            state.$attachmentInput.val('');
        }

        updateSendAvailability();
    }

    function clearAttachment() {
        state.pendingAttachment = null;

        if (state.$attachmentInput && state.$attachmentInput.length) {
            state.$attachmentInput.val('');
        }

        if (state.$attachmentName) {
            state.$attachmentName.text('');
        }

        if (state.$attachmentPreview) {
            state.$attachmentPreview.attr('hidden', 'hidden');
        }

        updateSendAvailability();
    }

    function resetComposer() {
        if (state.$input) {
            state.$input.val('');
        }

        clearAttachment();
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
        const hasAttachment = !!state.pendingAttachment;
        $button.prop('disabled', !(hasText || hasAttachment));
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

        const rawText = extractField(message, messageFields.text) || '';
        const timestamp = extractField(message, messageFields.timestamp) || new Date().toISOString();
        const sender = extractField(message, messageFields.sender) || '';
        const id = extractField(message, messageFields.id) || message?.tempId || '';
        const attachment = normaliseAttachment(message, rawText);
        const text = attachment ? (attachment.caption || attachment.fileName || '') : rawText;

        const isMine = determineIsMine(message, project, sender);

        return {
            id: id,
            text: text,
            rawText: rawText,
            timestamp: timestamp,
            sender: sender,
            isMine: isMine,
            attachment: attachment,
            optimistic: message.optimistic || false
        };
    }

    function normaliseAttachment(message, rawText) {
        if (message?.attachment || message?.Attachment) {
            return transformAttachment(message.attachment || message.Attachment);
        }

        return parseAttachmentPayload(rawText);
    }

    function parseAttachmentPayload(rawText) {
        if (typeof rawText !== 'string' || rawText.indexOf(ATTACHMENT_PREFIX) !== 0) {
            return null;
        }

        const json = rawText.substring(ATTACHMENT_PREFIX.length);
        try {
            const payload = JSON.parse(json);
            return transformAttachment(payload);
        } catch (error) {
            console.warn('[chatPanel] Failed to parse attachment payload', error);
            return null;
        }
    }

    function transformAttachment(source) {
        if (!source) {
            return null;
        }

        const fileName = source.fileName || source.name || '';
        const fileUrl = source.fileUrl || source.url || '';
        const caption = source.caption || source.title || '';
        const contentType = source.contentType || source.mimeType || '';
        const length = Number(source.length ?? source.size ?? 0) || 0;
        const isImage = typeof source.isImage === 'boolean'
            ? source.isImage
            : (contentType || '').toLowerCase().indexOf('image/') === 0;

        return {
            fileName: fileName,
            fileUrl: fileUrl,
            caption: caption,
            contentType: contentType,
            length: length,
            isImage: isImage,
            previewUrl: ''
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

    function formatBytes(bytes) {
        const value = Number(bytes);
        if (!Number.isFinite(value) || value <= 0) {
            return '';
        }

        const units = ['B', 'KB', 'MB', 'GB', 'TB'];
        let size = value;
        let unitIndex = 0;

        while (size >= 1024 && unitIndex < units.length - 1) {
            size /= 1024;
            unitIndex += 1;
        }

        const precision = size >= 10 || unitIndex === 0 ? 0 : 1;
        return `${size.toFixed(precision)} ${units[unitIndex]}`;
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
