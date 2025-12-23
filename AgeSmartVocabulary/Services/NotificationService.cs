using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using AgeSmartVocabulary.Data;

namespace AgeSmartVocabulary.Services
{
    public class NotificationService
    {
        private readonly DatabaseService _database;
        private const int DailyNotificationId = 1001;

        public NotificationService(DatabaseService database)
        {
            _database = database;
        }

        /// <summary>
        /// Schedule daily notification for word of the day
        /// </summary>
        public async Task<bool> ScheduleDailyNotificationAsync()
        {
            try
            {
                // Get user's preferred time
                var profile = await _database.GetUserProfileAsync();
                if (profile == null)
                    return false;

                var notificationTime = TimeSpan.Parse(profile.PreferredNotificationTime);

                // Cancel existing notification
                LocalNotificationCenter.Current.Cancel(DailyNotificationId);

                // Create notification request
                var notification = new NotificationRequest
                {
                    NotificationId = DailyNotificationId,
                    Title = "📚 Word of the Day",
                    Description = "Your daily word is ready! Tap to learn something new.",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Today.Add(notificationTime),
                        RepeatType = NotificationRepeat.Daily
                    },
                    Android = new AndroidOptions
                    {
                        AutoCancel = true,
                        Priority = AndroidPriority.High
                    }
                };

                await LocalNotificationCenter.Current.Show(notification);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Notification scheduling error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Show immediate test notification
        /// </summary>
        public async Task<bool> ShowTestNotificationAsync()
        {
            try
            {
                var notification = new NotificationRequest
                {
                    NotificationId = 9999,
                    Title = "🧪 Test Notification",
                    Description = "Notifications are working! You'll receive your daily word at the scheduled time.",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(3)
                    },
                    Android = new AndroidOptions
                    {
                        AutoCancel = true
                    }
                };

                await LocalNotificationCenter.Current.Show(notification);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Test notification error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cancel all notifications
        /// </summary>
        public void CancelAllNotifications()
        {
            try
            {
                LocalNotificationCenter.Current.CancelAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cancel notifications error: {ex.Message}");
            }
        }

        /// <summary>
        /// Request notification permissions (iOS/Android)
        /// </summary>
        public async Task<bool> RequestPermissionsAsync()
        {
            try
            {
                var hasPermission = await LocalNotificationCenter.Current.RequestNotificationPermission();
                return hasPermission;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Permission request error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if notifications are enabled
        /// </summary>
        public async Task<bool> AreNotificationsEnabledAsync()
        {
            try
            {
                var hasPermission = await LocalNotificationCenter.Current.RequestNotificationPermission();
                return hasPermission;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Check notification status error: {ex.Message}");
                return false;
            }
        }
    }
}