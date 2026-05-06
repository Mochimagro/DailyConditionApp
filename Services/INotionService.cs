using DailyConditionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public interface INotionService
    {
        Task<bool> AddDailyLogAsync(string token, string databaseId, DailyLogData logData);
        Task<TodayConditionResult?> GetTodayConditionAsync(string token, string databaseId, string dateString);
        Task<List<SleepScoreItem>> GetWeeklySleepScoresAsync(string token, string databaseId);

    }
}
