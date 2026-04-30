using CommunityToolkit.Maui;
using DailyConditionApp;
using DailyConditionApp.Services;
using DailyConditionApp.ViewModels;
using DailyConditionApp.Views;

public static class MauiProgram
{
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

        builder.Services.AddSingleton<MainView>();
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddTransient<DailyInputView>();
        builder.Services.AddTransient<DailyInputViewModel>();

        builder.Services.AddTransient<DailyConditionResultView>();
        builder.Services.AddTransient<DailyConditionResultViewModel>();

        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddSingleton<ISettingsService,SettingsService>();

        builder.Services.AddSingleton<IDialogService, DialogService>();

        builder.Services.AddHttpClient<IWeatherService, WeatherService>();
        return builder.Build();
    }
}