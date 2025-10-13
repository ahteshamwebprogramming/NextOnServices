(function ($) {
    'use strict';

    if (!$ || !$.fn) {
        return;
    }

    const PANEL_SELECTOR = '#projectChatPanel';
    const POLL_INTERVAL = 30000;
    const MAX_ATTACHMENT_SIZE = 10 * 1024 * 1024; // 10 MB
    const ALLOWED_ATTACHMENT_MIME_PREFIXES = ['image/'];
    const ALLOWED_ATTACHMENT_MIME_TYPES = [
        'application/pdf',
        'application/msword',
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
        'application/vnd.ms-excel',
        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        'text/plain'
    ];
    const ALLOWED_ATTACHMENT_EXTENSIONS = ['.jpg', '.jpeg', '.png', '.gif', '.pdf', '.doc', '.docx', '.xls', '.xlsx', '.txt'];

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
        $fileInput: null,
        $attachmentPreview: null,
        activeProject: null,
        pollTimer: null,
        isLoading: false,
        isSending: false
    };

    const messageFields = {
        text: ['message', 'text', 'body', 'content', 'note'],
        id: ['id', 'messageId', 'conversationMessageId', 'chatId'],
        timestamp: ['createdAt', 'createdOn', 'createdUtc', 'created', 'sentAt', 'timestamp', 'date', 'loggedAt'],
        sender: ['sender', 'from', 'author', 'owner', 'createdByName'],
        attachmentUrl: ['attachmentUrl', 'attachmentURL', 'attachmentPath', 'attachmentStoragePath'],
        attachmentName: ['attachmentOriginalFileName', 'attachmentOriginalName', 'attachmentName', 'attachmentFileName', 'originalFileName', 'fileName'],
        attachmentMimeType: ['attachmentMimeType', 'mimeType', 'contentType'],
        attachmentSize: ['attachmentSizeBytes', 'attachmentSize', 'sizeBytes', 'size']
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
        state.$fileInput = $panel.find('[data-chat-file]');
        state.$attachmentPreview = $panel.find('[data-chat-attachment-preview]');
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

        if (state.$fileInput) {
            state.$fileInput.on('change', function () {
                updateAttachmentPreview();
                updateSendAvailability();
            });
        }

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
        clearAttachmentSelection();
        updateAttachmentPreview();
        updateSendAvailability();

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
        state.isSending = false;
        resetStatus();
        clearAttachmentSelection();
        updateAttachmentPreview();
        updateSendAvailability();
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
        const $message = $('<div/>');
        populateMessageElement($message, message);

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

        populateMessageElement($existing, message);
    }

    function populateMessageElement($message, message) {
        if (!$message || !message) {
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

        $message.attr('class', classes.join(' '));
        $message.attr('data-message-id', message.id || '');
        $message.attr('data-temp-id', message.tempId || '');
        $message.data('chatMessage', message);

        const senderText = resolveSenderText(message);
        let $sender = $message.find('.chat-message__sender');
        if (senderText) {
            if (!$sender.length) {
                $sender = $('<div/>', { class: 'chat-message__sender' }).prependTo($message);
            }
            $sender.text(senderText);
        } else {
            $sender.remove();
        }

        let $bubble = $message.find('.chat-message__bubble');
        if (!$bubble.length) {
            $bubble = $('<div/>', { class: 'chat-message__bubble' }).appendTo($message);
        }
        populateBubble($bubble, message);

        const formattedTimestamp = message.timestamp ? formatTimestamp(message.timestamp) : '';
        let $meta = $message.find('.chat-message__meta');
        if (formattedTimestamp) {
            if (!$meta.length) {
                $meta = $('<div/>', { class: 'chat-message__meta' }).appendTo($message);
            }
            $meta.text(formattedTimestamp);
        } else {
            $meta.remove();
        }

        let $error = $message.find('.chat-message__error');
        if (message.error) {
            if (!$error.length) {
                $error = $('<div/>', { class: 'chat-message__error text-danger' }).appendTo($message);
            }
            $error.text(message.error);
        } else {
            $error.remove();
        }
    }

    function populateBubble($bubble, message) {
        if (!$bubble) {
            return;
        }

        $bubble.empty();

        if (message.text) {
            $('<div/>', { class: 'chat-message__text', text: message.text }).appendTo($bubble);
        }

        if (message.attachment) {
            renderAttachment($bubble, message.attachment);
        }

        if (!message.text && !message.attachment) {
            $bubble.append($('<span/>', { text: '' }));
        }
    }

    function renderAttachment($container, attachment) {
        if (!$container || !attachment) {
            return;
        }

        const url = attachment.url || attachment.storagePath || attachment.path || '';
        const name = attachment.name || getFileNameFromPath(url);
        const mimeType = (attachment.mimeType || '').toLowerCase();
        const size = Number.isFinite(attachment.size) ? attachment.size : null;
        const isImage = mimeType.startsWith('image/');

        const $wrapper = $('<div/>', { class: 'chat-message__attachment' });

        if (isImage && url) {
            const $link = $('<a/>', {
                href: url,
                target: '_blank',
                rel: 'noopener',
                class: 'chat-message__attachment-link chat-message__attachment-link--image'
            });

            $('<img/>', {
                src: url,
                alt: name || 'Attachment preview'
            }).appendTo($link);

            $wrapper.append($link);

            if (name) {
                $('<div/>', { class: 'chat-message__attachment-name', text: name }).appendTo($wrapper);
            }
        } else if (url) {
            $('<a/>', {
                href: url,
                target: '_blank',
                rel: 'noopener',
                class: 'chat-message__attachment-link',
                text: name || 'Download attachment'
            }).appendTo($wrapper);
        } else if (name) {
            $('<span/>', { class: 'chat-message__attachment-name', text: name }).appendTo($wrapper);
        }

        if (size) {
            $('<div/>', {
                class: 'chat-message__attachment-meta text-muted small',
                text: formatFileSize(size)
            }).appendTo($wrapper);
        }

        $container.append($wrapper);
    }

    function sendMessage() {
        const project = state.activeProject;
        if (!project || !state.$input) {
            return;
        }

        const value = state.$input.val();
        const trimmed = (value || '').trim();
        const attachmentFile = state.$fileInput && state.$fileInput[0] && state.$fileInput[0].files && state.$fileInput[0].files.length
            ? state.$fileInput[0].files[0]
            : null;

        if (!trimmed && !attachmentFile) {
            return;
        }

        const sendUrl = project.sendUrl;
        if (!sendUrl) {
            showStatus('Sending is disabled. Please contact support.', 'warning');
            return;
        }

        const attachmentValidation = validateAttachmentClient(attachmentFile);
        if (!attachmentValidation.valid) {
            showStatus(attachmentValidation.message, 'error');
            return;
        }

        const tempId = `tmp-${Date.now()}`;
        const optimisticMessage = {
            id: '',
            tempId: tempId,
            text: trimmed,
            timestamp: new Date().toISOString(),
            isMine: true,
            optimistic: true,
            attachment: attachmentFile ? {
                name: attachmentFile.name,
                size: attachmentFile.size,
                mimeType: attachmentFile.type || '',
                url: '',
                storagePath: ''
            } : null
        };

        appendMessage(optimisticMessage, { scroll: true });
        state.$input.val('');
        updateAttachmentPreview();

        state.isSending = true;
        updateSendAvailability();

        const formData = buildSendFormData(project, trimmed, attachmentFile);

        $.ajax({
            url: sendUrl,
            method: 'POST',
            dataType: 'json',
            processData: false,
            contentType: false,
            data: formData,
            xhr: function () {
                const xhr = $.ajaxSettings.xhr();
                if (xhr && xhr.upload) {
                    xhr.upload.addEventListener('progress', function (event) {
                        if (event.lengthComputable) {
                            const percent = Math.round((event.loaded / event.total) * 100);
                            showStatus(`Uploading… ${percent}%`, 'warning');
                        }
                    });
                }
                return xhr;
            }
        }).done(function (response) {
            const message = shapeMessage(response, project);
            message.tempId = tempId;
            message.optimistic = false;
            replaceOptimisticMessage(tempId, message);
            project.lastMessage = message.timestamp || new Date().toISOString();
            clearAttachmentSelection();
            updateAttachmentPreview();
            resetStatus();
        }).fail(function (xhr) {
            console.error('[chatPanel] Failed to send message', xhr);
            markOptimisticAsFailed(tempId, xhr);
        }).always(function () {
            state.isSending = false;
            updateSendAvailability();
        });
    }

    function buildSendFormData(project, message, attachmentFile) {
        const formData = new FormData();
        formData.append('pid', project.pid || '');

        const identifiers = {
            projectMappingId: normalizeId(project.projectMappingId),
            projectId: normalizeId(project.projectId),
            supplierId: normalizeId(project.supplierId)
        };

        Object.keys(identifiers).forEach(function (key) {
            if (identifiers[key] !== null && identifiers[key] !== undefined) {
                formData.append(key, identifiers[key]);
            }
        });

        if (message) {
            formData.append('message', message);
        }

        if (attachmentFile) {
            formData.append('attachment', attachmentFile);
        }

        return formData;
    }

    function validateAttachmentClient(file) {
        if (!file) {
            return { valid: true };
        }

        if (file.size <= 0) {
            return { valid: false, message: 'The selected attachment is empty.' };
        }

        if (file.size > MAX_ATTACHMENT_SIZE) {
            return { valid: false, message: `Attachments must be smaller than ${formatFileSize(MAX_ATTACHMENT_SIZE)}.` };
        }

        const mimeType = (file.type || '').toLowerCase();
        const extension = getFileExtension(file.name);
        const matchesMimeType = mimeType && (ALLOWED_ATTACHMENT_MIME_TYPES.indexOf(mimeType) >= 0 || ALLOWED_ATTACHMENT_MIME_PREFIXES.some(function (prefix) {
            return mimeType.indexOf(prefix) === 0;
        }));
        const matchesExtension = extension && ALLOWED_ATTACHMENT_EXTENSIONS.indexOf(extension) >= 0;

        if (!matchesMimeType && !matchesExtension) {
            return { valid: false, message: 'Unsupported attachment type. Allowed: images, PDF, text, or Office documents.' };
        }

        return { valid: true };
    }

    function getFileExtension(name) {
        if (!name) {
            return '';
        }

        const parts = name.split('.');
        if (parts.length <= 1) {
            return '';
        }

        return `.${parts.pop().toLowerCase()}`;
    }

    function getFileNameFromPath(path) {
        if (!path) {
            return '';
        }

        const normalised = path.replace(/\\/g, '/');
        const segments = normalised.split('/');
        return segments.pop() || '';
    }

    function formatFileSize(bytes) {
        if (!bytes || bytes <= 0) {
            return '0 B';
        }

        const units = ['B', 'KB', 'MB', 'GB'];
        let size = bytes;
        let unitIndex = 0;

        while (size >= 1024 && unitIndex < units.length - 1) {
            size /= 1024;
            unitIndex += 1;
        }

        return `${size.toFixed(unitIndex === 0 ? 0 : 1)} ${units[unitIndex]}`;
    }

    function updateAttachmentPreview() {
        if (!state.$attachmentPreview) {
            return;
        }

        const file = state.$fileInput && state.$fileInput[0] && state.$fileInput[0].files
            ? state.$fileInput[0].files[0]
            : null;

        state.$attachmentPreview.removeClass('text-danger');

        if (!file) {
            state.$attachmentPreview.text('');
            return;
        }

        const validation = validateAttachmentClient(file);
        if (!validation.valid) {
            state.$attachmentPreview.addClass('text-danger').text(validation.message);
            return;
        }

        const details = [file.name];
        if (file.size) {
            details.push(formatFileSize(file.size));
        }

        state.$attachmentPreview.text(details.join(' · '));
    }

    function clearAttachmentSelection() {
        if (state.$fileInput && state.$fileInput.length) {
            state.$fileInput.val('');
        }

        if (state.$attachmentPreview) {
            state.$attachmentPreview.removeClass('text-danger').text('');
        }
    }

    function markOptimisticAsFailed(tempId, xhr) {
        if (!state.$log) {
            return;
        }

        const $existing = state.$log.find(`[data-temp-id="${tempId}"]`).first();
        if (!$existing.length) {
            return;
        }

        const message = $.extend({}, $existing.data('chatMessage') || {});
        message.tempId = tempId;
        message.error = deriveErrorMessage(xhr);
        message.optimistic = false;
        if (typeof message.isMine !== 'boolean') {
            message.isMine = $existing.hasClass('chat-message--outgoing');
        }

        populateMessageElement($existing, message);
        showStatus(message.error, 'error');
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
        let hasAttachment = false;
        let attachmentValid = true;

        if (state.$fileInput && state.$fileInput[0] && state.$fileInput[0].files && state.$fileInput[0].files.length) {
            const file = state.$fileInput[0].files[0];
            hasAttachment = true;
            attachmentValid = validateAttachmentClient(file).valid;
        }

        const shouldDisable = state.isSending || (!hasText && !hasAttachment) || !attachmentValid;
        $button.prop('disabled', shouldDisable);
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
        const attachmentUrl = extractField(message, messageFields.attachmentUrl) || '';
        const attachmentName = extractField(message, messageFields.attachmentName) || '';
        const attachmentMimeType = extractField(message, messageFields.attachmentMimeType) || '';
        const attachmentSizeRaw = extractField(message, messageFields.attachmentSize);
        const storagePath = message?.attachmentStoragePath || '';

        let attachmentSize = null;
        if (attachmentSizeRaw !== null && attachmentSizeRaw !== undefined && attachmentSizeRaw !== '') {
            const parsedSize = Number(attachmentSizeRaw);
            attachmentSize = Number.isFinite(parsedSize) ? parsedSize : null;
        }

        let attachment = null;
        if (attachmentUrl || storagePath || attachmentName) {
            const url = attachmentUrl || storagePath || '';
            attachment = {
                url: url,
                storagePath: storagePath || '',
                name: attachmentName || getFileNameFromPath(url),
                mimeType: attachmentMimeType || '',
                size: attachmentSize
            };
        }

        const isMine = determineIsMine(message, project, sender);

        return {
            id: id,
            text: text,
            timestamp: timestamp,
            sender: sender,
            isMine: isMine,
            optimistic: message.optimistic || false,
            tempId: message.tempId || '',
            attachment: attachment
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

    function resolveSenderText(message) {
        if (!message) {
            return '';
        }

        const raw = message.sender ?? (message.isMine ? 'You' : '');
        if (raw === undefined || raw === null) {
            return '';
        }

        const text = String(raw).trim();
        return text;
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
