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
                .UseLocalNotification() // Add this line
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Services
            // Register Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<DatamuseApiService>();
            builder.Services.AddSingleton<DictionaryApiService>();
            builder.Services.AddSingleton<WordService>(); // Add this
            builder.Services.AddSingleton<NotificationService>();

            // Register ViewModels
            builder.Services.AddTransient<AgeSelectionViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            //builder.Services.AddTransient<WordDetailViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<ReviewListViewModel>();

            // Register Pages
            builder.Services.AddTransient<AgeSelectionPage>();
            builder.Services.AddTransient<HomePage>();
            //builder.Services.AddTransient<WordDetailPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ReviewListPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}