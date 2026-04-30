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
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty] private string _notionToken = string.Empty;
        [ObservableProperty] private string _databaseId = string.Empty;
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
            IsBusy = true;
            var notionSettings = await _settingsService.LoadNotionKeyAsync();
            NotionToken = notionSettings.token;
            DatabaseId = notionSettings.databaseId;
            var weatherSettings = await _settingsService.LoadWeatherSettingsAsync();
            WeatherApiKey = weatherSettings.ApiKey;
            Latitude = weatherSettings.Lat;
            Longitude = weatherSettings.Lon;
            IsBusy = false;
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            IsBusy = true;
            await _settingsService.SaveNotionSettingsAsync(NotionToken,DatabaseId);
            await _settingsService.SaveWeatherSettingsAsync(WeatherApiKey, Latitude, Longitude);
            // ここで Toast 等を出して保存完了を通知すると親切です
        
        await _dialogService.ShowToastAsync("保存しました");
            IsBusy = false;
        }
    }
}
