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
        public int Score { get; set; }

        // XAMLで表示しやすい形式にフォーマットするプロパティ
        public string FormattedDate => Date.ToString("MM/dd (ddd)");
    }
}
