using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Models;
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
        // リストのデータソース
        [ObservableProperty]
        private ObservableCollection<SleepScoreItem> _weeklyScores = new();

        public WeeklyResultsViewModel()
        {
            // プレビュー用に仮データを初期化時に読み込む（後でAPI連携に置き換え）
            LoadMockData();
        }

        [RelayCommand]
        public async Task LoadWeeklyDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // TODO: ここにNotion APIなどから過去7日分のデータを取得する処理を実装
                await Task.Delay(500); // 通信のモック
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadMockData()
        {
            var mockData = new List<SleepScoreItem>();
            DateTime today = DateTime.Now;

            // 過去7日分のモックデータを生成（最新日付が上に来るように降順で作成）
            for (int i = 0; i < 7; i++)
            {
                mockData.Add(new SleepScoreItem
                {
                    Date = today.AddDays(-i),
                    Score = Random.Shared.Next(60, 101) // 60〜100点のランダム
                });
            }

            WeeklyScores = new ObservableCollection<SleepScoreItem>(mockData);
        }
    }
}
