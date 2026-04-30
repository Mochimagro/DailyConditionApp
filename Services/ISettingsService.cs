using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public interface ISettingsService
    {
        Task SaveNotionSettingsAsync(string token, string databeseID);
        Task<(string token, string databaseId)> LoadNotionKeyAsync();

        Task SaveWeatherSettingsAsync(string apiKey, string lat, string lon);
        Task<(string ApiKey, string Lat, string Lon)> LoadWeatherSettingsAsync();
    }
}
