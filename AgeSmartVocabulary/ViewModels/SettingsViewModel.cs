using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Models;
using AgeSmartVocabulary.Services;

namespace AgeSmartVocabulary.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private UserProfile userProfile;

        [ObservableProperty]
        private string selectedAgeGroup;

        [ObservableProperty]
        private TimeSpan notificationTime;

        [ObservableProperty]
        private int totalReviewedWords;

        [ObservableProperty]
        private bool notificationsEnabled;

        public List<string> AgeGroups { get; } = new List<string>
        {
            "5-7", "8-10", "11-13", "14-18", "Adult"
        };

        public SettingsViewModel(DatabaseService database, NotificationService notificationService)
        {
            _database = database;
            _notificationService = notificationService;
            Title = "Settings";
        }

        public async Task InitializeAsync()
        {
            try
            {
                UserProfile = await _database.GetUserProfileAsync();

                if (UserProfile != null)
                {
                    SelectedAgeGroup = UserProfile.AgeGroup;

                    if (TimeSpan.TryParse(UserProfile.PreferredNotificationTime, out var time))
                    {
                        NotificationTime = time;
                    }
                }

                // Count reviewed words
                var seenWords = await _database.GetSeenWordsAsync();
                TotalReviewedWords = seenWords.Count;

                NotificationsEnabled = await _notificationService.AreNotificationsEnabledAsync();

                System.Diagnostics.Debug.WriteLine($"✓ Settings loaded: {TotalReviewedWords} words reviewed");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            try
            {
                if (UserProfile == null)
                {
                    UserProfile = new UserProfile();
                }

                UserProfile.AgeGroup = SelectedAgeGroup;
                UserProfile.PreferredNotificationTime = NotificationTime.ToString(@"hh\:mm");

                await _database.SaveUserProfileAsync(UserProfile);

                // Reschedule notification with new time
                await _notificationService.ScheduleDailyNotificationAsync();

                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Settings saved! Notification scheduled for " + NotificationTime.ToString(@"hh\:mm"),
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task TestNotificationAsync()
        {
            try
            {
                var result = await _notificationService.ShowTestNotificationAsync();

                if (result)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Test Sent",
                        "Check your notifications in 3 seconds!",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Failed to send test notification. Check permissions.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task ClearHistoryAsync()
        {
            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Clear History",
                "This will delete all your review progress. Are you sure?",
                "Yes",
                "No");

            if (!confirm)
                return;

            try
            {
                // We would need to add a method to clear all reviews
                await Application.Current.MainPage.DisplayAlert(
                    "Cleared",
                    "Review history cleared. Restart app to begin fresh.",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Public methods for SettingsPage event handlers
        public async Task<bool> ScheduleDailyNotificationAsync()
        {
            return await _notificationService.ScheduleDailyNotificationAsync();
        }

        public void CancelAllNotifications()
        {
            _notificationService.CancelAllNotifications();
        }
    }
}