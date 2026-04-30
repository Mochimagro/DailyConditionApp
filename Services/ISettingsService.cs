using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public interface ISettingsService
    {
        Task SaveNotionApiKeyAsync(string apiKey);

        Task<string> LoadNotionApiKeyAsync();

        Task SaveWeatherSettingsAsync(string apiKey, string lat, string lon);
        Task<(string ApiKey, string Lat, string Lon)> LoadWeatherSettingsAsync();
    }
}
