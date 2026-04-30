using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.ViewModels
{
    public partial class DailyInputViewModel : ObservableObject
    {
        // 文字列で受け取り、必要時に double.Parse 等で変換する構成にしています。
        [ObservableProperty]
        private string sleepHours;

        [ObservableProperty]
        private string sleepEfficiency;

        [ObservableProperty]
        private string weather;

        [ObservableProperty]
        private string pressure;

        [ObservableProperty]
        private string windSpeed;

        // Picker の選択肢
        public IReadOnlyList<string> WeatherOptions { get; } = new[]
        {
            "晴れ",
            "曇り",
            "雨",
            "雪",
            "霧"
        };
    }
}
