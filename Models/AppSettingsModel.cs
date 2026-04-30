using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Models
{
    class AppSettingsModel
    {
        public string NotionApiKey { get; set; } = string.Empty;

        // 今後増えたらここに追加していく
        // public string DatabaseId { get; set; } = string.Empty;
    }
}
