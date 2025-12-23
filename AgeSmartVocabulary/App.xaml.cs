using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Views;
using AgeSmartVocabulary.ViewModels;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace AgeSmartVocabulary
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Creating Window ===");

                // Initialize with simple page first
                MainPage = new ContentPage
                {
                    Content = new ActivityIndicator
                    {
                        IsRunning = true,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };

                // Then load actual app asynchronously
                Dispatcher.Dispatch(async () => await InitializeAppAsync());

                return base.CreateWindow(activationState);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreateWindow Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");

                // Show error page
                MainPage = new ContentPage
                {
                    BackgroundColor = Colors.White,
                    Content = new VerticalStackLayout
                    {
                        Padding = 30,
                        Spacing = 20,
                        Children =
                        {
                            new Label
                            {
                                Text = "App Failed to Start",
                                FontSize = 24,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Colors.Red,
                                HorizontalTextAlignment = TextAlignment.Center
                            },
                            new Label
                            {
                                Text = ex.Message,
                                FontSize = 14,
                                TextColor = Colors.Black,
                                HorizontalTextAlignment = TextAlignment.Center
                            }
                        }
                    }
                };

                return base.CreateWindow(activationState);
            }
        }

        private async Task InitializeAppAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Initializing App ===");

                // Check if user profile exists
                var database = new DatabaseService();
                System.Diagnostics.Debug.WriteLine("✓ Database created");

                var profile = await database.GetUserProfileAsync();
                System.Diagnostics.Debug.WriteLine($"✓ Profile check: {(profile == null ? "New user" : "Existing user")}");

                if (profile == null)
                {
                    // First launch - show age selection
                    System.Diagnostics.Debug.WriteLine("→ Navigating to AgeSelection");
                    var viewModel = new AgeSelectionViewModel(database);
                    MainPage = new NavigationPage(new AgeSelectionPage(viewModel));
                }
                else
                {
                    // User exists - show home
                    System.Diagnostics.Debug.WriteLine("→ Navigating to AppShell");
                    MainPage = new AppShell();
                }

                System.Diagnostics.Debug.WriteLine("✓ App initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ InitializeApp Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    MainPage = new ContentPage
                    {
                        BackgroundColor = Colors.White,
                        Content = new ScrollView
                        {
                            Content = new VerticalStackLayout
                            {
                                Padding = 30,
                                Spacing = 20,
                                Children =
                                {
                                    new Label
                                    {
                                        Text = "⚠️ Initialization Error",
                                        FontSize = 24,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Colors.Orange,
                                        HorizontalTextAlignment = TextAlignment.Center
                                    },
                                    new Label
                                    {
                                        Text = ex.Message,
                                        FontSize = 14,
                                        TextColor = Colors.Black,
                                        HorizontalTextAlignment = TextAlignment.Center
                                    },
                                    new Label
                                    {
                                        Text = ex.StackTrace,
                                        FontSize = 10,
                                        TextColor = Colors.Gray,
                                        LineBreakMode = LineBreakMode.WordWrap
                                    }
                                }
                            }
                        }
                    };
                });
            }
        }
    }
}