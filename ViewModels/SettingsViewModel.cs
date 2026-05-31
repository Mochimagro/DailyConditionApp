using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;
using Microsoft.Maui.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace DailyConditionApp.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty] private string _notionToken = string.Empty;
        [ObservableProperty] private string _databaseId = string.Empty;
        [ObservableProperty] private string _weatherApiKey = string.Empty;
        [ObservableProperty] private string _latitude = string.Empty;
        [ObservableProperty] private string _longitude = string.Empty;
        [ObservableProperty] private bool _sundayChecked;
        [ObservableProperty] private bool _mondayChecked;
        [ObservableProperty] private bool _tuesdayChecked;
        [ObservableProperty] private bool _wednesdayChecked;
        [ObservableProperty] private bool _thursdayChecked;
        [ObservableProperty] private bool _fridayChecked;
        [ObservableProperty] private bool _saturdayChecked;

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
            // ご褒美曜日の読み込み
            var rewardDays = await _settingsService.LoadRewardDaysAsync();
            // DayOfWeek の int 値 (Sunday=0 ... Saturday=6) を想定
            SundayChecked = rewardDays.Contains((int)System.DayOfWeek.Sunday);
            MondayChecked = rewardDays.Contains((int)System.DayOfWeek.Monday);
            TuesdayChecked = rewardDays.Contains((int)System.DayOfWeek.Tuesday);
            WednesdayChecked = rewardDays.Contains((int)System.DayOfWeek.Wednesday);
            ThursdayChecked = rewardDays.Contains((int)System.DayOfWeek.Thursday);
            FridayChecked = rewardDays.Contains((int)System.DayOfWeek.Friday);
            SaturdayChecked = rewardDays.Contains((int)System.DayOfWeek.Saturday);
            IsBusy = false;
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            IsBusy = true;
            await _settingsService.SaveNotionSettingsAsync(NotionToken, DatabaseId);
            await _settingsService.SaveWeatherSettingsAsync(WeatherApiKey, Latitude, Longitude);

            // ご褒美曜日の保存
            var days = new List<int>();
            if (SundayChecked) days.Add((int)System.DayOfWeek.Sunday);
            if (MondayChecked) days.Add((int)System.DayOfWeek.Monday);
            if (TuesdayChecked) days.Add((int)System.DayOfWeek.Tuesday);
            if (WednesdayChecked) days.Add((int)System.DayOfWeek.Wednesday);
            if (ThursdayChecked) days.Add((int)System.DayOfWeek.Thursday);
            if (FridayChecked) days.Add((int)System.DayOfWeek.Friday);
            if (SaturdayChecked) days.Add((int)System.DayOfWeek.Saturday);
            await _settingsService.SaveRewardDaysAsync(days);

            await _dialogService.ShowToastAsync("保存しました");
            IsBusy = false;
        }
    }
}
