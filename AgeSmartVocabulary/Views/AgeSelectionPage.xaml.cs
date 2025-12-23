using AgeSmartVocabulary.ViewModels;

namespace AgeSmartVocabulary.Views
{
    public partial class AgeSelectionPage : ContentPage
    {
        public AgeSelectionPage(AgeSelectionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnAgeGroupSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var viewModel = (AgeSelectionViewModel)BindingContext;

                // Automatically proceed when selection is made
                await Task.Delay(200); // Small delay for visual feedback
                await viewModel.SaveAgeGroupCommand.ExecuteAsync(null);
            }
        }
    }
}