using CommunityToolkit.Mvvm.ComponentModel;

namespace AgeSmartVocabulary.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title;

        public BaseViewModel()
        {
        }
    }
}