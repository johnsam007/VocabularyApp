using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Services;

namespace AgeSmartVocabulary.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;
        private readonly WordService _wordService;

        [ObservableProperty]
        private WordData currentWord;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string userAgeGroup;

        public HomeViewModel(DatabaseService database, WordService wordService)
        {
            _database = database;
            _wordService = wordService;
            Title = "Word of the Day";
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;

            try
            {
                System.Diagnostics.Debug.WriteLine("=== HomeViewModel Initialize ===");

                // Get user profile
                var profile = await _database.GetUserProfileAsync();
                if (profile != null)
                {
                    UserAgeGroup = profile.AgeGroup;
                    System.Diagnostics.Debug.WriteLine($"✓ User Age Group: {UserAgeGroup}");
                }

                // Load today's word
                await LoadTodayWordAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Initialize error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadTodayWordAsync()
        {
            try
            {
                IsLoading = true;
                System.Diagnostics.Debug.WriteLine("→ Loading today's word...");

                CurrentWord = await _wordService.GetTodayWordAsync(UserAgeGroup);

                if (CurrentWord == null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "No Words",
                        "Unable to fetch word. Check your internet connection.",
                        "OK");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Loaded word: {CurrentWord.Word}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LoadTodayWord error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load word: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task MarkAsKnownAsync()
        {
            if (CurrentWord == null) return;

            try
            {
                await _wordService.MarkWordAsKnownAsync(CurrentWord.Word);
                await Application.Current.MainPage.DisplayAlert("Great!", "You've marked this word as known! 🎉", "OK");
                await LoadTodayWordAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task MarkAsReviseAsync()
        {
            if (CurrentWord == null) return;

            try
            {
                await _wordService.MarkWordForRevisionAsync(CurrentWord.Word);
                await Application.Current.MainPage.DisplayAlert("No Problem!", "We'll review this word again tomorrow. 📝", "OK");
                await LoadTodayWordAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}