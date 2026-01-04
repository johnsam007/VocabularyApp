using Microsoft.Extensions.Logging;
using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Services;
using AgeSmartVocabulary.ViewModels;
using AgeSmartVocabulary.Views;
using Plugin.LocalNotification;

namespace AgeSmartVocabulary
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configure HttpClient (IMPORTANT!)
            builder.Services.AddHttpClient();

            // Register Services (Singleton - one instance for entire app)
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<DatamuseApiService>();
            builder.Services.AddSingleton<DictionaryApiService>();
            builder.Services.AddSingleton<WordService>();
            builder.Services.AddSingleton<NotificationService>();

            // Register ViewModels (Transient - new instance each time)
            builder.Services.AddTransient<AgeSelectionViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<ReviewListViewModel>();

            // Register Pages (Transient - new instance each time)
            builder.Services.AddTransient<AgeSelectionPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ReviewListPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}