using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Models;

namespace AgeSmartVocabulary.ViewModels
{
    public partial class ReviewListViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;

        [ObservableProperty]
        private List<ReviewSchedule> reviewSchedules;

        [ObservableProperty]
        private bool isLoading;

        public ReviewListViewModel(DatabaseService database)
        {
            _database = database;
            Title = "Review Schedule";
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;

            try
            {
                var schedules = await _database.GetAllPendingReviewsAsync();
                ReviewSchedules = schedules.OrderBy(x => x.NextReviewDate).ToList();

                System.Diagnostics.Debug.WriteLine($"✓ Loaded {ReviewSchedules.Count} pending reviews");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReviewList error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RefreshListAsync()
        {
            await InitializeAsync();
        }
    }
}