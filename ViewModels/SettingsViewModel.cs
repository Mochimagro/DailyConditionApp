using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;
using Microsoft.Maui.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyConditionApp.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty] private string _notionApiKey = string.Empty;
        [ObservableProperty] private string _weatherApiKey = string.Empty;
        [ObservableProperty] private string _latitude = string.Empty;
        [ObservableProperty] private string _longitude = string.Empty;

        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        public SettingsViewModel(ISettingsService settingsService, IDialogService dialogService)
        {
            _settingsService = settingsService;
            _dialogService = dialogService;
            LoadSettingsAsync();
        }

        private async void LoadSettingsAsync()
        {
            NotionApiKey = await _settingsService.LoadNotionApiKeyAsync();

            var weatherSettings = await _settingsService.LoadWeatherSettingsAsync();
            WeatherApiKey = weatherSettings.ApiKey;
            Latitude = weatherSettings.Lat;
            Longitude = weatherSettings.Lon;
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            await _settingsService.SaveNotionApiKeyAsync(NotionApiKey);
            await _settingsService.SaveWeatherSettingsAsync(WeatherApiKey, Latitude, Longitude);
            // ここで Toast 等を出して保存完了を通知すると親切です
        
        await _dialogService.ShowToastAsync("保存しました");
        }
    }
}
