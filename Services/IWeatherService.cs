using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public interface IWeatherService
    {
        Task<WeatherCondition?> GetWeatherAsync(string apiKey, string lat, string lon);
    }
}
