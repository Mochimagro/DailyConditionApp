using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace DailyConditionApp.Models
{
    public partial class SleepScoreItem : ObservableObject
    {
        public DateTime Date { get; set; }

        // Notion 側で "睡眠スコア" がある場合はそこを優先して使えるようにする
        private int? _score;
        public int? Score
        {
            get
            {
                if (_score.HasValue) return Math.Max(0, Math.Min(100, _score.Value));
                if (Coefficient.HasValue)
                {
                    // Coefficient が 0..1 の想定なので 100 倍して整数化
                    var calc = (int)Math.Round(Coefficient.Value * 100);
                    return Math.Max(0, Math.Min(100, calc));
                }
                return null;
            }
            set
            {
                _score = value;
            }
        }

        public double? Coefficient { get; set; }

        public string FormattedDate => Date.ToString("MM/dd (ddd)");

        // Viewで表示するための文字列プロパティ
        public string DisplayScore => Score.HasValue ? $"{Score.Value} 点" : "-- 点";

        public string DisplayCoefficiented => Coefficient.HasValue ? $"{Coefficient.Value} " : "--";

        // 色分け用プロパティ（点数によって色を変える）
        public Color ScoreColor => Score.HasValue
            ? (Score.Value >= 80 ? Color.FromArgb("#42A5F5") : Color.FromArgb("#777777"))
            : Color.FromArgb("#555555");

        // チェックボックス用のプロパティ（UIバインド、変更通知あり）
        [ObservableProperty]
        private bool isChecked;
    }
}
