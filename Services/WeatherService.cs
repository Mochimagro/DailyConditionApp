using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    /// <summary>
    /// 取得したデータをまとめるクラス
    /// </summary>
    public class WeatherCondition
    {
        public string Description { get; set; } = string.Empty;
        public double Pressure { get; set; } // hPa
        public double WindSpeed { get; set; } // m/s
        public string CustomStatus { get; set; } = string.Empty;
    }


    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherCondition?> GetWeatherAsync(string apiKey, string lat, string lon)
        {
            string url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={lat},{lon}&lang=ja&aqi=no";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var current = doc.RootElement.GetProperty("current");

                // GASロジックに必要な値を取得
                int code = current.GetProperty("condition").GetProperty("code").GetInt32();
                int cloud = current.GetProperty("cloud").GetInt32();
                double precip = current.GetProperty("precip_mm").GetDouble();

                return new WeatherCondition
                {
                    Description = current.GetProperty("condition").GetProperty("text").GetString() ?? "",
                    Pressure = current.GetProperty("pressure_mb").GetDouble(),
                    WindSpeed = current.GetProperty("wind_kph").GetDouble(),

                    // GASから移植した判定ロジックを適用
                    CustomStatus = InterpretWeatherCondition(code, cloud, precip)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GASコードをC#へ移植
        private string InterpretWeatherCondition(int code, int cloud, double precip)
        {
            // 1. 【最優先】台風 (雷系)
            int[] stormCodes = { 1087, 1273, 1276, 1279, 1282 };
            if (stormCodes.Contains(code)) return "台風";

            // 2. 【最優先】雪
            int[] snowCodes = { 1066, 1114, 1117, 1210, 1213, 1216, 1219, 1222, 1225, 1255, 1258 };
            if (snowCodes.Contains(code)) return "雪";

            // 3. 降水量 10mm 以上は「雨」
            if (precip >= 10) return "雨";

            // 4. 雲の量 50% 未満は「晴れ」
            if (cloud < 50) return "晴れ";

            // 5. それ以外は「曇り」
            return "曇り";
        }
    }
        
}
