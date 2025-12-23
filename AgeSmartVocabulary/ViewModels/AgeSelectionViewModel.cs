using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Models;
using AgeSmartVocabulary.Helpers;
using AgeSmartVocabulary.Services;

namespace AgeSmartVocabulary.ViewModels
{
    public partial class AgeSelectionViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;

        [ObservableProperty]
        private List<AgeGroupOption> ageGroups;

        [ObservableProperty]
        private AgeGroupOption selectedAgeGroup;

        public AgeSelectionViewModel(DatabaseService database)
        {
            _database = database;
            Title = "Select Your Age Group";
            LoadAgeGroups();
        }

        private void LoadAgeGroups()
        {
            AgeGroups = new List<AgeGroupOption>
            {
                new AgeGroupOption { Value = "5-7", Label = "🎈 Early Learner (5-7 years)", Description = "Simple, everyday words" },
                new AgeGroupOption { Value = "8-10", Label = "📚 Elementary (8-10 years)", Description = "Common school vocabulary" },
                new AgeGroupOption { Value = "11-13", Label = "🎓 Middle School (11-13 years)", Description = "Academic words" },
                new AgeGroupOption { Value = "14-18", Label = "🏆 High School (14-18 years)", Description = "Advanced vocabulary" },
                new AgeGroupOption { Value = "Adult", Label = "💼 Adult", Description = "Professional & rare words" }
            };
        }

        [RelayCommand]
        private async Task SaveAgeGroupAsync()
        {
            if (SelectedAgeGroup == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select an age group", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                // Save user profile
                var profile = new UserProfile
                {
                    AgeGroup = SelectedAgeGroup.Value,
                    PreferredNotificationTime = "07:00"
                };

                await _database.SaveUserProfileAsync(profile);

                // Setup notifications
                var notificationService = new NotificationService(_database);
                await notificationService.RequestPermissionsAsync();
                await notificationService.ScheduleDailyNotificationAsync();

                // Navigate to Home
                await Shell.Current.GoToAsync("///HomePage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    // Helper class for age group options
    public class AgeGroupOption
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}