using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;

namespace DailyConditionApp.ViewModels
{
    public partial class DailyConditionResultViewModel : BaseViewModel
    {
        private readonly INotionService _notionService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty] private string _environmentScoreText = "--";
        [ObservableProperty] private string _conditionCommentText = "読み込み中...";

        [ObservableProperty] private int _sleepScore;
        [ObservableProperty] private int _pressureScore;
        [ObservableProperty] private int _weatherScore;
        [ObservableProperty] private int _windScore;

        public DailyConditionResultViewModel(INotionService notionService, ISettingsService settingsService)
        {
            _notionService = notionService;
            _settingsService = settingsService;
        }

        [RelayCommand]
        public async Task LoadResultAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var notionSettings = await _settingsService.LoadNotionKeyAsync();
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                var result = await _notionService.GetTodayConditionAsync(notionSettings.token, notionSettings.databaseId, today);

                if (result != null)
                {
                    EnvironmentScoreText = Math.Ceiling(result.EnvironmentScore).ToString();
                    ConditionCommentText = result.ConditionComment;

                    // 各スコアは Notion から取得した数値を小数点切り上げで表示
                    SleepScore = (int)Math.Ceiling(result.SleepScore);
                    PressureScore = (int)Math.Ceiling(result.PressureScore);
                    WeatherScore = (int)Math.Ceiling(result.WeatherScore);
                    WindScore = (int)Math.Ceiling(result.WindScore);
                }
                else
                {
                    ConditionCommentText = "本日のデータはまだ登録されていません。";
                    // スコアはデフォルト 0 に戻す（任意）
                    SleepScore = 0;
                    PressureScore = 0;
                    WeatherScore = 0;
                    WindScore = 0;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
