using AgeSmartVocabulary.ViewModels;
using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Services;

namespace AgeSmartVocabulary.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            var database = new DatabaseService();
            var notificationService = new NotificationService(database);

            _viewModel = new SettingsViewModel(database, notificationService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        private async void OnNotificationToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                var scheduled = await _viewModel.ScheduleDailyNotificationAsync();
                if (scheduled)
                {
                    await DisplayAlert("Enabled", "Daily notifications are now ON", "OK");
                }
            }
            else
            {
                _viewModel.CancelAllNotifications();
                await DisplayAlert("Disabled", "Daily notifications are now OFF", "OK");
            }
        }
    }
}