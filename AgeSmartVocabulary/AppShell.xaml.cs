using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Services;
using AgeSmartVocabulary.ViewModels;

namespace AgeSmartVocabulary
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            try
            {
                InitializeComponent();

                // Register routes for navigation
               // Routing.RegisterRoute(nameof(Views.WordDetailPage), typeof(Views.WordDetailPage));

                System.Diagnostics.Debug.WriteLine("✓ AppShell created successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ AppShell Error: {ex.Message}");
                throw;
            }
        }
    }
}