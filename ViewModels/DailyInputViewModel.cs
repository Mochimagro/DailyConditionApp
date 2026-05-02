using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly INotionService _notionService;

        // 文字列で受け取り、必要時に double.Parse 等で変換する構成にしています。
        [ObservableProperty]
        private string _date;
        
        [ObservableProperty]
        private TimeSpan _sleepTime;

        [ObservableProperty]
        private int _sleepHour;

        [ObservableProperty]
        private int _sleepMinute;

        [ObservableProperty]
        private int _sleepEfficiency = 100;

        [ObservableProperty]
        private string _weather;

        [ObservableProperty]
        private string _pressure;

        [ObservableProperty]
        private string _windSpeed;

        [ObservableProperty]
        private ObservableCollection<int> _pickerPerNumberItems;

        public DailyInputViewModel(ISettingsService settingsService, IWeatherService weatherService,IDialogService dialogService,INotionService notionService)
        {
            _settingsService = settingsService;
            _weatherService = weatherService;
            _dialogService = dialogService;
            _notionService = notionService;

            Date = DateTime.Now.ToString("yyyy-MM-dd");

            PickerPerNumberItems = new ObservableCollection<int>(Enumerable.Range(0, 101).Reverse()); // 1~100の選択肢を用意

            // 初期値を SleepTime から同期
            SleepHour = SleepTime.Hours;
            SleepMinute = SleepTime.Minutes;
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

        // 時間ドラム用のデータ
        public IReadOnlyList<int> Hours { get; } = Enumerable.Range(0, 24).ToArray();
        public IReadOnlyList<int> Minutes { get; } = Enumerable.Range(0, 60).ToArray();

        // SleepTime が変更されたら Hour/Minute を更新
        partial void OnSleepTimeChanged(TimeSpan value)
        {
            if (SleepHour != value.Hours) SleepHour = value.Hours;
            if (SleepMinute != value.Minutes) SleepMinute = value.Minutes;
        }

        // Hour が変更されたら SleepTime を更新
        partial void OnSleepHourChanged(int value)
        {
            var newTs = new TimeSpan(value, SleepMinute, 0);
            if (SleepTime != newTs) SleepTime = newTs;
        }

        // Minute が変更されたら SleepTime を更新
        partial void OnSleepMinuteChanged(int value)
        {
            var newTs = new TimeSpan(SleepHour, value, 0);
            if (SleepTime != newTs) SleepTime = newTs;
        }

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
            if (IsBusy) return;

            try
            {
                IsBusy = true; // ぐるぐる表示オン

                // 1. 設定からトークンとデータベースIDを読み込む
                // （※ SettingsService に DatabaseId の保存・読み込みを追加しておく必要があります）
                var notionSettings = await _settingsService.LoadNotionKeyAsync();
                string notionToken = notionSettings.token;
                string databaseId = notionSettings.databaseId;

                if (string.IsNullOrEmpty(notionToken) || string.IsNullOrEmpty(databaseId))
                {
                    // エラー通知（APIキー未設定）
                    await _dialogService.ShowToastAsync("Notion APIキーやデータベースIDが設定されていません。設定画面から見直してください。");
                    return;
                }

                // 2. 文字列の入力値を数値に変換 (バリデーションを兼ねる)
                _ = double.TryParse(Pressure, out double pressureDiff);
                _ = double.TryParse(WindSpeed, out double windSpeed);

                // 3. Notionに送るデータを用意
                var logData = new DailyLogData(
                    Date: DateTime.Now.ToString("yyyy-MM-dd"), // 日本時間で取得する場合は調整が必要
                    WeatherLabel: Weather,
                    SleepTime: SleepTime.TotalHours, // TimeSpanを時間数に変換
                    SleepEfficiency:(double)SleepEfficiency / 100,
                    PressureDiff: pressureDiff,
                    WindSpeed: windSpeed
                // RelatedPageId: "取得したIDがあればここに入れる"
                );

                // 4. NotionServiceを呼び出して送信
                bool isSuccess = await _notionService.AddDailyLogAsync(notionToken, databaseId, logData);

                if (isSuccess)
                {
                    await _dialogService.ShowToastAsync("Notionへの保存に成功しました");
                }
                else
                {
                    await _dialogService.ShowToastAsync("Notionへの保存に失敗しました。APIキーやデータベースIDを確認してください。");
                }
            }
            finally
            {
                IsBusy = false; // ぐるぐる表示オフ
            }

        }
    }
}
