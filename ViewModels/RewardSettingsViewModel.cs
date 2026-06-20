using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;
using System.Collections.Generic;

namespace DailyConditionApp.ViewModels
{
    public partial class RewardSettingsViewModel : BaseViewModel
    {
        [ObservableProperty] private bool _sundayChecked;
        [ObservableProperty] private bool _mondayChecked;
        [ObservableProperty] private bool _tuesdayChecked;
        [ObservableProperty] private bool _wednesdayChecked;
        [ObservableProperty] private bool _thursdayChecked;
        [ObservableProperty] private bool _fridayChecked;
        [ObservableProperty] private bool _saturdayChecked;
        [ObservableProperty] private string _rewardText = string.Empty;

        private readonly ISettingsService _settingsService;
        private bool _isInitializing = false;

        public RewardSettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            LoadAsync();
        }

        private async void LoadAsync()
        {
            _isInitializing = true;
            IsBusy = true;
            var days = await _settingsService.LoadRewardDaysAsync();
            SundayChecked = days.Contains((int)System.DayOfWeek.Sunday);
            MondayChecked = days.Contains((int)System.DayOfWeek.Monday);
            TuesdayChecked = days.Contains((int)System.DayOfWeek.Tuesday);
            WednesdayChecked = days.Contains((int)System.DayOfWeek.Wednesday);
            ThursdayChecked = days.Contains((int)System.DayOfWeek.Thursday);
            FridayChecked = days.Contains((int)System.DayOfWeek.Friday);
            SaturdayChecked = days.Contains((int)System.DayOfWeek.Saturday);
            RewardText = await _settingsService.LoadRewardTextAsync() ?? string.Empty;
            IsBusy = false;
            _isInitializing = false;
        }

        partial void OnSundayCheckedChanged(bool value) => SaveDaysIfReady();
        partial void OnMondayCheckedChanged(bool value) => SaveDaysIfReady();
        partial void OnTuesdayCheckedChanged(bool value) => SaveDaysIfReady();
        partial void OnWednesdayCheckedChanged(bool value) => SaveDaysIfReady();
        partial void OnThursdayCheckedChanged(bool value) => SaveDaysIfReady();
        partial void OnFridayCheckedChanged(bool value) => SaveDaysIfReady();
        partial void OnSaturdayCheckedChanged(bool value) => SaveDaysIfReady();

        private void SaveDaysIfReady()
        {
            if (_isInitializing) return;
            var days = new List<int>();
            if (SundayChecked) days.Add((int)System.DayOfWeek.Sunday);
            if (MondayChecked) days.Add((int)System.DayOfWeek.Monday);
            if (TuesdayChecked) days.Add((int)System.DayOfWeek.Tuesday);
            if (WednesdayChecked) days.Add((int)System.DayOfWeek.Wednesday);
            if (ThursdayChecked) days.Add((int)System.DayOfWeek.Thursday);
            if (FridayChecked) days.Add((int)System.DayOfWeek.Friday);
            if (SaturdayChecked) days.Add((int)System.DayOfWeek.Saturday);
            _ = _settingsService.SaveRewardDaysAsync(days);
        }

        partial void OnRewardTextChanged(string value)
        {
            if (_isInitializing) return;
            _ = _settingsService.SaveRewardTextAsync(RewardText);
        }
    }
}
