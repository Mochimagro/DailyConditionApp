using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Models;
using DailyConditionApp.Services;
using Microcharts;
using SkiaSharp;
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

        [ObservableProperty]
        private Chart _weeklyChart;

        // チェックしたアイテムの平均表示用テキスト
        [ObservableProperty]
        private string _checkedAverageText = "--";

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
                        // Score = matchedItem?.Score, // データがあればScoreを入れ、なければnull
                        Coefficient = matchedItem?.Coefficient,
                        IsChecked = false // 初期は未チェック
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

                    // Microcharts 用の BarChart を作成
                    try
                    {
                        var ordered = completeWeeklyData.OrderBy(d => d.Date).ToList();
                        var entries = ordered.Select(i =>
                        {
                            var score = i.Score ?? 0;
                            // 80以上を水色 ("#03A9F4"), 80未満を灰色 ("#9E9E9E") に変更
                            var color = score >= 80 ? SKColor.Parse("#03A9F4") : SKColor.Parse("#9E9E9E");
                            var entry = new ChartEntry((float)score)
                            {
                                Label = i.Date.ToString("MM/dd"),
                                ValueLabel = score.ToString(),
                                Color = color
                            };
                            return entry;
                        }).ToArray();

                        // カスタムの BarChart を使って、80点のラインを描画し、値テキストを棒の上部に表示する
                        WeeklyChart = new DailyConditionApp.Controls.CustomBarChart
                        {
                            Entries = entries,
                            MaxValue = 100,
                            MinValue = 0,
                            LabelTextSize = 18,
                            // 背景を少し暗めにして棒とのコントラストを上げる
                            BackgroundColor = SKColor.Parse("#EFEFEF"),
                            Threshold = 80f,
                            // ThresholdColor = SKColors.White
                        };
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"WeeklyResultsViewModel: chart init error: {ex}");
                    }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void CalculateCheckedAverage()
        {
            var selected = WeeklyScores.Where(w => w.IsChecked && w.Score.HasValue).Select(w => w.Score.Value).ToList();
            if (selected == null || selected.Count == 0)
            {
                CheckedAverageText = "--";
                return;
            }

            var avg = (int)Math.Round(selected.Average());
            CheckedAverageText = $"平均: {avg} 点 ({selected.Count} 件)";
        }
    }
}
