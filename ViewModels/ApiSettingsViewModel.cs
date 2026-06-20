using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;

namespace DailyConditionApp.ViewModels
{
    public partial class ApiSettingsViewModel : BaseViewModel
    {
        [ObservableProperty] private string _notionToken = string.Empty;
        [ObservableProperty] private string _databaseId = string.Empty;
        [ObservableProperty] private string _weatherApiKey = string.Empty;
        [ObservableProperty] private string _latitude = string.Empty;
        [ObservableProperty] private string _longitude = string.Empty;

        private readonly ISettingsService _settingsService;
        private bool _isInitializing = false;

        public ApiSettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            LoadAsync();
        }

        private async void LoadAsync()
        {
            _isInitializing = true;
            IsBusy = true;
            var notion = await _settingsService.LoadNotionKeyAsync();
            NotionToken = notion.token;
            DatabaseId = notion.databaseId;
            var weather = await _settingsService.LoadWeatherSettingsAsync();
            WeatherApiKey = weather.ApiKey;
            Latitude = weather.Lat;
            Longitude = weather.Lon;
            IsBusy = false;
            _isInitializing = false;
        }

        partial void OnNotionTokenChanged(string value)
        {
            if (_isInitializing) return;
            _ = _settingsService.SaveNotionSettingsAsync(NotionToken, DatabaseId);
        }

        partial void OnDatabaseIdChanged(string value)
        {
            if (_isInitializing) return;
            _ = _settingsService.SaveNotionSettingsAsync(NotionToken, DatabaseId);
        }

        partial void OnWeatherApiKeyChanged(string value)
        {
            if (_isInitializing) return;
            _ = _settingsService.SaveWeatherSettingsAsync(WeatherApiKey, Latitude, Longitude);
        }

        partial void OnLatitudeChanged(string value)
        {
            if (_isInitializing) return;
            _ = _settingsService.SaveWeatherSettingsAsync(WeatherApiKey, Latitude, Longitude);
        }

        partial void OnLongitudeChanged(string value)
        {
            if (_isInitializing) return;
            _ = _settingsService.SaveWeatherSettingsAsync(WeatherApiKey, Latitude, Longitude);
        }
    }
}
