using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public interface ISettingService
    {
        Task SaveApiKeyAsync(string apiKey);

        Task<string> LoadApiKeyAsync();
    }
}
