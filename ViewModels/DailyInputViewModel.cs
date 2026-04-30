using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.ViewModels
{
    public partial class DailyInputViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IWeatherService _weatherService;
        private readonly IDialogService _dialogService;

        // 文字列で受け取り、必要時に double.Parse 等で変換する構成にしています。
        [ObservableProperty]
        private string _date;
        
        [ObservableProperty]
        private TimeSpan _sleepTime;

        [ObservableProperty]
        private string _sleepEfficiency;

        [ObservableProperty]
        private string _weather;

        [ObservableProperty]
        private string _pressure;

        [ObservableProperty]
        private string _windSpeed;

        public DailyInputViewModel(ISettingsService settingsService, IWeatherService weatherService,IDialogService dialogService)
        {
            _settingsService = settingsService;
            _weatherService = weatherService;
            _dialogService = dialogService;

            Date = DateTime.Now.ToString("yyyy-MM-dd");
        }

        // Picker の選択肢
        public IReadOnlyList<string> WeatherOptions { get; } = new[]
        {
            "晴れ",
            "曇り",
            "雨",
            "雪",
            "台風"
        };

        [RelayCommand]
        private async Task FetchWeatherAsync()
        {
            IsBusy = true;
            var settings = await _settingsService.LoadWeatherSettingsAsync();

            if (string.IsNullOrEmpty(settings.ApiKey) || string.IsNullOrEmpty(settings.Lat) || string.IsNullOrEmpty(settings.Lon))
            {
                // 設定が足りない場合のエラーハンドリング（Toast等で警告を出すと良いです）
                await _dialogService.ShowToastAsync("APIの入力が不完全です。設定画面からWeatherAPIを見直して下さい");

                IsBusy = false;
                return;
            }

            var condition = await _weatherService.GetWeatherAsync(settings.ApiKey, settings.Lat, settings.Lon);

            if (condition != null)
            {
                Weather = condition.CustomStatus; // 例: "曇りがち"
                Pressure = condition.Pressure.ToString(); // 例: "1013"
                                                          // 時速(km/h) を 秒速(m/s) に変換して小数点第1位まで表示
                                                          // 1 km/h = 1000m / 3600s ≒ 1 / 3.6
                double windMs = condition.WindSpeed / 3.6;
                WindSpeed = windMs.ToString("F1");
            }
            else
            {
                await _dialogService.ShowToastAsync("天気情報の取得に失敗しました。APIキーや位置情報を確認してください。");
            }

            IsBusy = false;

        }

        [RelayCommand]
        private async Task WriteToNotionAsync()
        {
            IsBusy = true;
            if(string.IsNullOrEmpty(SleepTime.ToString()) || string.IsNullOrEmpty(SleepEfficiency) || string.IsNullOrEmpty(Weather) || string.IsNullOrEmpty(Pressure) || string.IsNullOrEmpty(WindSpeed))
            {
                await _dialogService.ShowToastAsync("全ての項目を入力してください。");
                
                IsBusy = false;
                return;
            }

            await _dialogService.ShowToastAsync("Notionに書き込む予定です。");
            IsBusy = false;
        }
    }
}
