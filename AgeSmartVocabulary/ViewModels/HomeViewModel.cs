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

                // Show success message
                await Application.Current.MainPage.DisplayAlert(
                    "✅ Great!",
                    $"You know '{CurrentWord.Word}'! Keep going!",
                    "Next");

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

                // Show revision message
                await Application.Current.MainPage.DisplayAlert(
                    "📝 No Problem!",
                    $"We'll review '{CurrentWord.Word}' again tomorrow.",
                    "Next");

                await LoadTodayWordAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task TestConnectionAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Testing Internet Connection ===");

                // Test 1: Check MAUI Connectivity
                var connectivity = Connectivity.Current.NetworkAccess;
                System.Diagnostics.Debug.WriteLine($"1. MAUI Connectivity: {connectivity}");

                // Test 2: Try simple HTTP request
                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

                System.Diagnostics.Debug.WriteLine("2. Testing Google...");
                var googleResponse = await httpClient.GetAsync("https://www.google.com");
                System.Diagnostics.Debug.WriteLine($"   Google Status: {googleResponse.StatusCode}");

                System.Diagnostics.Debug.WriteLine("3. Testing Datamuse API...");
                var datamuseResponse = await httpClient.GetAsync("https://api.datamuse.com/words?sp=test&max=1");
                System.Diagnostics.Debug.WriteLine($"   Datamuse Status: {datamuseResponse.StatusCode}");

                System.Diagnostics.Debug.WriteLine("4. Testing Dictionary API...");
                var dictResponse = await httpClient.GetAsync("https://api.dictionaryapi.dev/api/v2/entries/en/hello");
                System.Diagnostics.Debug.WriteLine($"   Dictionary Status: {dictResponse.StatusCode}");

                await Application.Current.MainPage.DisplayAlert(
                    "Connection Test Passed!",
                    $"✅ Google: {googleResponse.StatusCode}\n" +
                    $"✅ Datamuse: {datamuseResponse.StatusCode}\n" +
                    $"✅ Dictionary: {dictResponse.StatusCode}",
                    "OK");

                System.Diagnostics.Debug.WriteLine("=== All Tests Passed ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Connection Test Failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");

                await Application.Current.MainPage.DisplayAlert(
                    "Connection Test Failed",
                    $"Error: {ex.Message}\n\nCheck Output window for details.",
                    "OK");
            }
        }
    }
}