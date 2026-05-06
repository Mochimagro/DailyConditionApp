using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Models
{
    public class SleepScoreItem
    {
        public DateTime Date { get; set; }
        public int? Score { get; set; } // int? に変更（データがない場合を考慮）

        public string FormattedDate => Date.ToString("MM/dd (ddd)");

        // Viewで表示するための文字列プロパティを追加
        public string DisplayScore => Score.HasValue ? $"{Score.Value} 点" : "-- 点";

        // 色分け用プロパティ（オプション：点数によって色を変える）
        public Color ScoreColor => Score.HasValue
            ? (Score.Value >= 80 ? Color.FromArgb("#42A5F5") : Color.FromArgb("#FFFFFF"))
            : Color.FromArgb("#555555");
    }
}
