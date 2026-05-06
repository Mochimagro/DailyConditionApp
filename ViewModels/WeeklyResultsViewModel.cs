using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Models;
using DailyConditionApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.ViewModels
{
    public partial class WeeklyResultsViewModel : BaseViewModel
    {
        private readonly INotionService _notionService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private ObservableCollection<SleepScoreItem> _weeklyScores = new();

        // コンストラクタでServiceを受け取るように修正
        public WeeklyResultsViewModel(INotionService notionService, ISettingsService settingsService)
        {
            _notionService = notionService;
            _settingsService = settingsService;
        }

        [RelayCommand]
        public async Task LoadWeeklyDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var notionKey = await _settingsService.LoadNotionKeyAsync();

                // Notionから生データを取得
                var fetchedData = await _notionService.GetWeeklySleepScoresAsync(notionKey.token, notionKey.databaseId);

                // カレンダー通りの7日分のリストを生成する
                var completeWeeklyData = new List<SleepScoreItem>();
                DateTime today = DateTime.Now.Date;

                for (int i = 0; i < 7; i++)
                {
                    DateTime targetDate = today.AddDays(-i);

                    // Notionの取得結果の中に、対象日のデータがあるか探す
                    var matchedItem = fetchedData.FirstOrDefault(d => d.Date == targetDate);

                    completeWeeklyData.Add(new SleepScoreItem
                    {
                        Date = targetDate,
                        Score = matchedItem?.Score // データがあればScoreを入れ、なければnull
                    });
                }

                // メインスレッドでUIを更新
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeeklyScores.Clear();
                    foreach (var item in completeWeeklyData)
                    {
                        WeeklyScores.Add(item);
                    }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
