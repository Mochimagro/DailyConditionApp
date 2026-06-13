using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;
using DailyConditionApp.Services;

namespace DailyConditionApp.ViewModels
{
    public partial class DailyConditionResultViewModel : BaseViewModel
    {
        private readonly INotionService _notionService;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private string _environmentScoreText = "--";
        [ObservableProperty] private string _conditionCommentText = "読み込み中...";
        [ObservableProperty] private bool _isRewardAvailable;
        [ObservableProperty] private string _rewardText = "ご褒美可能！！";

        // 表示用（0〜100 の整数値）
        [ObservableProperty] private int _sleepScore;
        private string _averageSleepScoreText = "";
        public string AverageSleepScoreText
        {
            get => _averageSleepScoreText;
            set => SetProperty(ref _averageSleepScoreText, value);
        }

        private int _averageSleepScoreValue;
        public int AverageSleepScoreValue
        {
            get => _averageSleepScoreValue;
            set
            {
                if (SetProperty(ref _averageSleepScoreValue, value))
                {
                    AverageSleepScoreProgress = Math.Clamp(value / 100.0, 0.0, 1.0);
                    // Update color
                    if (value <= 49)
                        AverageSleepScoreColor = Color.FromArgb("#F44336"); // red
                    else if (value <= 79)
                        AverageSleepScoreColor = Color.FromArgb("#FFD54F"); // yellow
                    else
                        AverageSleepScoreColor = Color.FromArgb("#4CAF50"); // green
                }
            }
        }

        private double _averageSleepScoreProgress;
        public double AverageSleepScoreProgress
        {
            get => _averageSleepScoreProgress;
            set => SetProperty(ref _averageSleepScoreProgress, value);
        }

        private Color _averageSleepScoreColor = Colors.Gray;
        public Color AverageSleepScoreColor
        {
            get => _averageSleepScoreColor;
            set => SetProperty(ref _averageSleepScoreColor, value);
        }
        [ObservableProperty] private double _pressureCoefficient;
        [ObservableProperty] private double _weatherCoefficient;
        [ObservableProperty] private double _windCoefficient;

        // 当日の Notion データが存在するか（表示制御用）
        [ObservableProperty] private bool _hasTodayData = false;

        public DailyConditionResultViewModel(INotionService notionService, ISettingsService settingsService, IDialogService dialogService)
        {
            _notionService = notionService;
            _settingsService = settingsService;
            _dialogService = dialogService;
        }

        [RelayCommand]
        public async Task LoadResultAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // ご褒美テキストを読み込む
                try
                {
                    var rtext = await _settingsService.LoadRewardTextAsync();
                    RewardText = string.IsNullOrWhiteSpace(rtext) ? "ご褒美可能！！" : rtext;
                }
                catch
                {
                    RewardText = "ご褒美可能！！";
                }

                var notionSettings = await _settingsService.LoadNotionKeyAsync();
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                var result = await _notionService.GetTodayConditionAsync(notionSettings.token, notionSettings.databaseId, today);

                if (result != null)
                {
                    HasTodayData = true;

                    EnvironmentScoreText = Math.Ceiling(result.EnvironmentScore).ToString();
                    ConditionCommentText = result.ConditionComment;

                    // 各スコアは Notion から取得した数値を小数点切り上げで表示
                    SleepScore = (int)(result.SleepCoefficient * 100);
                    PressureCoefficient = result.PressureCoefficient;
                    WeatherCoefficient = result.WeatherCoefficient;
                    WindCoefficient = result.WindCoefficient;

                    // 平均スコア（本日を含む過去3日間）の算出
                    try
                    {
                        await ComputeAverageSleepScoreAsync(notionSettings.token, notionSettings.databaseId);
                    }
                    catch
                    {
                        AverageSleepScoreText = "";
                    }

                }
                else
                {
                    HasTodayData = false;

                    ConditionCommentText = "本日のデータはまだ登録されていません。";

                    // スコアはデフォルト 0 に戻す（任意）
                    EnvironmentScoreText = "--";
                    SleepScore = 0;
                    PressureCoefficient = 0;
                    WeatherCoefficient = 0;
                    WindCoefficient = 0;
                    AverageSleepScoreText = "";
                    IsRewardAvailable = false;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ComputeAverageSleepScoreAsync(string token, string databaseId)
        {
            try
            {
                var list = await _notionService.GetWeeklySleepScoresAsync(token, databaseId);

                // 集計対象の日付（今日を含めた過去3日）
                var today = DateTime.Now.Date;
                var validDates = new[] { today, today.AddDays(-1), today.AddDays(-2) };

                var selected = list.Where(x => x.Coefficient.HasValue && validDates.Contains(x.Date.Date)).ToList();

                // 今日は必ず表示されている想定だが、念のためチェック
                if (selected.Count == 0)
                {
                    AverageSleepScoreText = "";
                    AverageSleepScoreValue = 0;
                    return;
                }

                // Coefficient * 100 -> 整数スコア、平均は四捨五入
                var scores = selected.Select(x => (int)Math.Round(x.Coefficient.Value * 100)).ToList();
                var avg = (int)Math.Round(scores.Average());

                AverageSleepScoreValue = avg;
                AverageSleepScoreText = $"過去3日間の平均睡眠スコア: {avg} 点";

                var rewardDays = await _settingsService.LoadRewardDaysAsync();
                var todayInt = (int)DateTime.Now.DayOfWeek;

                if (rewardDays.Contains(todayInt) && avg >= 80)
                {
                    IsRewardAvailable = true;
                    // トースト通知
                    // await _dialogService.ShowToastAsync("合格日");
                }
                else
                {
                    IsRewardAvailable = false;
                }
            }
            catch
            {
                AverageSleepScoreText = "";
                AverageSleepScoreValue = 0;
                IsRewardAvailable = false;
            }
        }

        [RelayCommand]
        private void PushScoreNotification()
        {
            // Android版のNotificationServiceを取得
            var notificationService = DependencyService.Get<INotificationService>();

            if (notificationService != null)
            {
                string title = $"スコア：{EnvironmentScoreText} ";
                string message = $"{ConditionCommentText}";

                notificationService.ShowPersistentNotification(title, message);
            }
        }
    }
}
