using CommunityToolkit.Maui;
using DailyConditionApp;
using DailyConditionApp.Services;
using DailyConditionApp.ViewModels;
using DailyConditionApp.Views;
using DailyConditionApp.Views.Controls;

public static class MauiProgram
{
    public static IServiceProvider CurrentServiceProvider { get; private set; }
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            // After initializing the .NET MAUI Community Toolkit, optionally add additional fonts
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddHttpClient();

        // Continue initializing your .NET MAUI App here


        builder.Services.AddSingleton<ISettingsService, SettingsService>();

        builder.Services.AddSingleton<IDialogService, DialogService>();

        builder.Services.AddHttpClient<INotionService, NotionService>();

        builder.Services.AddHttpClient<IWeatherService, WeatherService>();

#if ANDROID
        DependencyService.Register<INotificationService, DailyConditionApp.Platforms.Android.AndroidNotificationService>();
#endif

        builder.Services.AddTransient<MainView>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<DailyInputView>();
        builder.Services.AddTransient<DailyInputViewModel>();

        builder.Services.AddSingleton<DailyConditionResultView>();
        builder.Services.AddSingleton<DailyConditionResultViewModel>();

        builder.Services.AddTransient<PostedDailyView>();
        builder.Services.AddTransient<PostedDailyViewModel>();

        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddTransient<WeeklyResultsView>();
        builder.Services.AddTransient<WeeklyResultsViewModel>();

        builder.Services.AddTransient<LoadingView>();
        var app = builder.Build();

        CurrentServiceProvider = app.Services;

        return app;
    }
}