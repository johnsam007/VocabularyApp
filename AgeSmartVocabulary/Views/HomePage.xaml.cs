using AgeSmartVocabulary.ViewModels;
using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Services;

namespace AgeSmartVocabulary.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel _viewModel;

        public HomePage()
        {
            InitializeComponent();

            // Create dependencies
            var database = new DatabaseService();
            var datamuseApi = new DatamuseApiService();
            var dictionaryApi = new DictionaryApiService();
            var wordService = new WordService(datamuseApi, dictionaryApi, database);

            _viewModel = new HomeViewModel(database, wordService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            System.Diagnostics.Debug.WriteLine("→ HomePage appearing, refreshing word...");
            await _viewModel.InitializeAsync();
        }
    }
}