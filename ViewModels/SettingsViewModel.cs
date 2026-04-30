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
        [ObservableProperty]
        private string _apiKey;

        private readonly ISettingService _settingsService;
        private readonly IDialogService _dialogService;

        public SettingsViewModel(ISettingService settingsService, IDialogService dialogService)
        {
            _settingsService = settingsService;
            _dialogService = dialogService;
            LoadData(); // 画面が開いた時にロードする
        }

        private async void LoadData()
        {
            var loadString = await _settingsService.LoadApiKeyAsync();

            ApiKey = string.IsNullOrEmpty(loadString) ? "" : loadString; 
        }

        [RelayCommand]
        private async Task SaveSettings()
        {
            // 入力された文字をJSONに保存
            await _settingsService.SaveApiKeyAsync(ApiKey);
            // 必要に応じて「保存しました」ポップアップなどを出す

                await _dialogService.ShowToastAsync("保存しました");
        }
    }
}
