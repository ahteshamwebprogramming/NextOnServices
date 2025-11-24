(function ($) {
    'use strict';

    if (!$ || !$.fn) {
        return;
    }

    const NOTIFICATIONS_URL = '/VT/Supplier/GetAllProjectNotifications';
    const NOTIFICATIONS_PAGE_SIZE = 50;

    const state = {
        currentPage: 1,
        pageSize: NOTIFICATIONS_PAGE_SIZE,
        unreadOnly: false,
        filterType: 'all', // 'all', 'unread', 'changes'
        totalCount: 0,
        totalPages: 0,
        isLoading: false,
        lastNotificationTime: null
    };

    $(document).ready(function () {
        initializeNotificationsPage();
    });

    function initializeNotificationsPage() {
        // Load initial notifications
        loadNotifications();

        // Filter change handlers
        $('input[name="notificationFilter"]').on('change', function () {
            const filterValue = $(this).val();
            state.filterType = filterValue;
            state.unreadOnly = filterValue === 'unread';
            state.currentPage = 1;
            loadNotifications();
        });

        // Refresh handler
        $('#refreshNotifications').on('click', function (e) {
            e.preventDefault();
            loadNotifications();
        });

        // Pagination handlers
        $('#prevPage').on('click', function (e) {
            e.preventDefault();
            if (state.currentPage > 1) {
                state.currentPage--;
                loadNotifications();
            }
        });

        $('#nextPage').on('click', function (e) {
            e.preventDefault();
            if (state.currentPage < state.totalPages) {
                state.currentPage++;
                loadNotifications();
            }
        });

        // Notification item click handler - opens project chat
        $(document).on('click', '.notification-item-page', function (e) {
            e.preventDefault();
            const projectMappingId = $(this).data('project-mapping-id');
            if (projectMappingId) {
                openProjectChat(projectMappingId);
            }
        });

        // Mark notification as read when clicked
        $(document).on('click', '.notification-item-page.unread', function () {
            const notificationId = $(this).data('notification-id');
            if (notificationId) {
                markNotificationAsRead(notificationId, $(this));
            }
        });

        // Auto-refresh every 15 seconds to check for new notifications
        setInterval(function() {
            if (!state.isLoading) {
                checkForNewNotifications(); // Check for new notifications and update badge
            }
        }, 15000);
    }

    function checkForNewNotifications() {
        // Check for new notifications since last check
        const since = state.lastNotificationTime ? new Date(state.lastNotificationTime) : null;
        
        $.ajax({
            url: NOTIFICATIONS_URL,
            method: 'GET',
            dataType: 'json',
            data: {
                page: 1,
                pageSize: 10, // Only check recent ones
                unreadOnly: false,
                since: since ? since.toISOString() : null
            },
            cache: false
        }).done(function (response) {
            if (response && response.notifications) {
                const newNotifications = response.notifications || [];
                
                if (newNotifications.length > 0) {
                    // Sort by time (newest first)
                    newNotifications.sort((a, b) => {
                        const timeA = new Date(a.createdUtc || 0);
                        const timeB = new Date(b.createdUtc || 0);
                        return timeB - timeA;
                    });

                    // Update last notification time
                    if (newNotifications[0] && newNotifications[0].createdUtc) {
                        state.lastNotificationTime = newNotifications[0].createdUtc;
                    }

                    // Update notification badge (only alert - no popups)
                    if (typeof window.updateNotificationBadge === 'function') {
                        window.updateNotificationBadge();
                    }

                    // Reload full notifications if on first page
                    if (state.currentPage === 1) {
                        loadNotifications(true);
                    }
                }
            }
        }).fail(function (xhr, status, error) {
            console.error('[centralizedNotifications] Failed to check for new notifications:', error);
        });
    }

    function loadNotifications(silent = false) {
        if (state.isLoading) {
            return;
        }

        state.isLoading = true;
        if (!silent) {
            showLoading();
        }

        const params = {
            page: state.currentPage,
            pageSize: state.pageSize,
            unreadOnly: state.unreadOnly
        };

        $.ajax({
            url: NOTIFICATIONS_URL,
            method: 'GET',
            dataType: 'json',
            data: params,
            cache: false
        }).done(function (response) {
            console.log('[centralizedNotifications] Response received:', response);
            if (response && response.notifications !== undefined) {
                updateNotifications(response);
            } else {
                console.warn('[centralizedNotifications] Unexpected response format:', response);
                if (!silent) {
                    showEmptyState('Failed to load notifications. Please try again.');
                }
            }
        }).fail(function (xhr, status, error) {
            console.error('[centralizedNotifications] Failed to fetch notifications:', {
                status: xhr.status,
                statusText: xhr.statusText,
                responseText: xhr.responseText,
                error: error
            });
            if (!silent) {
                showEmptyState('Failed to load notifications. Please try again.');
            }
        }).always(function () {
            state.isLoading = false;
            if (!silent) {
                hideLoading();
            }
        });
    }

    function updateNotifications(response) {
        let notifications = Array.isArray(response.notifications) ? response.notifications : [];
        const pagination = response.pagination || {};

        // Update last notification time on initial load
        if (!state.lastNotificationTime && notifications.length > 0) {
            const firstNotification = notifications[0];
            if (firstNotification.createdUtc) {
                state.lastNotificationTime = firstNotification.createdUtc;
            }
        }

        // Filter by project changes if needed
        if (state.filterType === 'changes') {
            notifications = notifications.filter(n => n.isProjectChange === true);
        }

        state.totalCount = pagination.totalCount || 0;
        state.totalPages = pagination.totalPages || 0;

        // Update pagination UI
        updatePagination();

        // Render notifications
        renderNotifications(notifications);

        // Update notification badge
        if (typeof window.updateNotificationBadge === 'function') {
            window.updateNotificationBadge();
        }
    }

    function renderNotifications(notifications) {
        const $container = $('#notificationsContainer');
        $container.empty();

        if (!notifications || notifications.length === 0) {
            let emptyMessage = 'No notifications found';
            if (state.filterType === 'unread') {
                emptyMessage = 'No unread notifications';
            } else if (state.filterType === 'changes') {
                emptyMessage = 'No project change notifications';
            }
            showEmptyState(emptyMessage);
            return;
        }

        const $list = $('<ul/>', { class: 'notification-list' }).appendTo($container);

        notifications.forEach(function (notification) {
            const isUnread = !notification.isRead;
            const isProjectChange = notification.isProjectChange === true || 
                                    (notification.message || '').indexOf('[PROJECT_UPDATE]') === 0;
            
            let message = notification.message || '';
            if (isProjectChange && message.indexOf('[PROJECT_UPDATE]') === 0) {
                message = message.substring('[PROJECT_UPDATE]'.length).trim();
            }

            const projectName = notification.projectName || notification.pid || 'Unknown Project';
            const country = notification.country || '';
            const senderName = notification.senderName || 'System';
            const timeAgo = formatTimeAgo(notification.createdUtc);

            const $item = $('<li/>', {
                class: 'notification-item-page' + (isUnread ? ' unread' : '') + (isProjectChange ? ' project-change' : ''),
                'data-project-mapping-id': notification.projectMappingId,
                'data-notification-id': notification.id
            }).appendTo($list);

            // Header
            const $header = $('<div/>', { class: 'notification-item-header' }).appendTo($item);
            
            const $title = $('<div/>', { class: 'notification-item-title' }).appendTo($header);

            // Icon
            if (isProjectChange) {
                $('<i/>', {
                    class: 'fas fa-edit text-info mr-2',
                    title: 'Project change'
                }).appendTo($title);
            } else {
                $('<i/>', {
                    class: 'fas fa-comment text-primary mr-2',
                    title: 'New message'
                }).appendTo($title);
            }

            $('<span/>', {
                class: 'notification-item-project' + (isProjectChange ? ' project-change-icon' : ''),
                text: projectName
            }).appendTo($title);

            if (country) {
                $('<span/>', {
                    class: 'text-muted ml-2',
                    text: `· ${country}`
                }).appendTo($title);
            }

            if (isUnread) {
                $('<span/>', {
                    class: 'notification-badge' + (isProjectChange ? ' project-change' : ''),
                    text: 'New'
                }).appendTo($title);
            }

            // Message
            if (message) {
                $('<div/>', {
                    class: 'notification-item-message',
                    text: message
                }).appendTo($item);
            }

            // Meta
            const $meta = $('<div/>', {
                class: 'notification-item-meta'
            }).appendTo($item);

            if (senderName) {
                $('<span/>', {
                    class: 'notification-item-sender',
                    text: senderName
                }).appendTo($meta);
            }

            if (timeAgo) {
                $('<span/>', {
                    class: 'notification-item-time',
                    text: timeAgo
                }).appendTo($meta);
            }
        });

        // Show pagination if needed
        if (state.totalPages > 1) {
            $('#notificationPagination').show();
        } else {
            $('#notificationPagination').hide();
        }
    }

    function updatePagination() {
        const $pageInfo = $('#pageInfo');
        $pageInfo.text(`Page ${state.currentPage} of ${state.totalPages || 1} (${state.totalCount} total)`);

        $('#prevPage').prop('disabled', state.currentPage <= 1);
        $('#nextPage').prop('disabled', state.currentPage >= state.totalPages);
    }

    function showEmptyState(message) {
        const $container = $('#notificationsContainer');
        $container.html(`
            <div class="empty-notifications">
                <i class="fas fa-bell-slash"></i>
                <p class="mb-0">${message || 'No notifications'}</p>
            </div>
        `);
        $('#notificationPagination').hide();
    }

    function showLoading() {
        const $container = $('#notificationsContainer');
        $container.html(`
            <div class="empty-notifications">
                <i class="fas fa-spinner fa-spin"></i>
                <p class="mb-0">Loading notifications...</p>
            </div>
        `);
        $('#notificationPagination').hide();
    }

    function hideLoading() {
        // Loading is hidden when notifications are rendered
    }

    function formatTimeAgo(timestamp) {
        if (!timestamp) {
            return '';
        }

        try {
            // Handle UTC timestamp from server - ensure it's parsed correctly
            let date;
            if (typeof timestamp === 'string') {
                // If timestamp ends with Z, it's UTC. If not, treat as UTC and add Z
                if (timestamp.indexOf('Z') === -1 && timestamp.indexOf('+') === -1 && timestamp.indexOf('-', 10) === -1) {
                    // No timezone indicator, assume UTC
                    date = new Date(timestamp + (timestamp.indexOf('T') !== -1 ? '' : 'T00:00:00') + 'Z');
                } else {
                    date = new Date(timestamp);
                }
            } else {
                date = new Date(timestamp);
            }

            // Validate the date
            if (isNaN(date.getTime())) {
                return '';
            }

            const now = new Date();
            const diffMs = now - date;
            
            // Handle negative differences (future dates) - shouldn't happen but just in case
            if (diffMs < 0) {
                return 'Just now';
            }

            const diffMins = Math.floor(diffMs / 60000);
            const diffHours = Math.floor(diffMs / 3600000);
            const diffDays = Math.floor(diffMs / 86400000);

            if (diffMins < 1) {
                return 'Just now';
            } else if (diffMins < 60) {
                return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
            } else if (diffHours < 24) {
                return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
            } else if (diffDays < 7) {
                return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
            } else {
                // For older dates, show full date and time in user's local timezone
                const options = { 
                    year: 'numeric', 
                    month: 'short', 
                    day: 'numeric',
                    hour: '2-digit', 
                    minute: '2-digit',
                    hour12: true
                };
                return date.toLocaleString(undefined, options);
            }
        } catch (err) {
            console.error('[centralizedNotifications] Error formatting time:', err, timestamp);
            return '';
        }
    }

    function markNotificationAsRead(notificationId, $item) {
        // This would mark a single notification as read
        // For now, clicking will just open the chat and it will be marked as read when viewing
        $item.removeClass('unread');
    }

    function openProjectChat(projectMappingId) {
        // Find the chat trigger button for this project and click it
        const $trigger = $(`.project-chat-trigger[data-project-id="${projectMappingId}"]`);
        if ($trigger.length) {
            $trigger.trigger('click');
        } else {
            // Reload to Dashboard and try to open chat
            window.location.href = `/VT/Supplier/Dashboard?openChat=${projectMappingId}`;
        }
    }

    // Expose functions if needed
    window.refreshCentralizedNotifications = function() {
        loadNotifications();
    };

})(window.jQuery);

