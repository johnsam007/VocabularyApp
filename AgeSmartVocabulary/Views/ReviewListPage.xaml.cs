using AgeSmartVocabulary.ViewModels;
using AgeSmartVocabulary.Data;

namespace AgeSmartVocabulary.Views
{
    public partial class ReviewListPage : ContentPage
    {
        private readonly ReviewListViewModel _viewModel;

        public ReviewListPage()
        {
            InitializeComponent();

            var database = new DatabaseService();
            _viewModel = new ReviewListViewModel(database);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}