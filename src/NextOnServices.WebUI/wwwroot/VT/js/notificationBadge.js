(function ($) {
    'use strict';

    if (!$ || !$.fn) {
        return;
    }

    const NOTIFICATION_COUNT_URL = '/VT/Supplier/GetUnreadNotificationCount';

    let notificationBadgeInterval = null;

    function updateNotificationBadge() {
        $.ajax({
            url: NOTIFICATION_COUNT_URL,
            method: 'GET',
            dataType: 'json',
            cache: false
        }).done(function (response) {
            if (response && typeof response.count === 'number') {
                const count = response.count || 0;
                const $badge = $('#navbarNotificationBadge');
                
                if (count > 0) {
                    $badge.text(count > 99 ? '99+' : count.toString()).show();
                } else {
                    $badge.hide();
                }
            }
        }).fail(function (xhr, status, error) {
            console.error('[notificationBadge] Failed to fetch notification count:', error);
        });
    }

    function initializeNotificationBadge() {
        // Initial load
        updateNotificationBadge();

        // Update every 10 seconds
        if (notificationBadgeInterval) {
            clearInterval(notificationBadgeInterval);
        }

        notificationBadgeInterval = setInterval(function() {
            updateNotificationBadge();
        }, 10000);
    }

    // Initialize when document is ready
    $(document).ready(function() {
        initializeNotificationBadge();
    });

    // Expose function to manually update badge
    window.updateNotificationBadge = updateNotificationBadge;

    // Update badge when page becomes visible (user switches tabs)
    document.addEventListener('visibilitychange', function() {
        if (!document.hidden) {
            updateNotificationBadge();
        }
    });

})(window.jQuery);

