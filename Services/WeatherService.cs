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
        public double AvgPressureDiff { get; set; } // 9:00-18:00の平均気圧差（hPa）
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
            // forecast.json に変更し、days=1 を指定することで1時間ごとのデータ(hour配列)を取得可能にする
            string url = $"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={lat},{lon}&days=1&lang=ja&aqi=no&alerts=no";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 「現在」のデータ
                var current = root.GetProperty("current");
                // 「当日」の予報データ（1時間ごとの配列を含む）
                var forecastDay = root.GetProperty("forecast").GetProperty("forecastday")[0];
                var hours = forecastDay.GetProperty("hour").EnumerateArray();

                // 1. 9:00から18:00の3時間毎のデータを抽出
                var targetHours = hours.Where(h =>
                {
                    var timeStr = h.GetProperty("time").GetString();
                    if (DateTime.TryParse(timeStr, out var time))
                    {
                        return time.Hour >= 9 && time.Hour <= 18 && time.Hour % 3 == 0;
                    }
                    return false;
                }).ToList();

                /*
                // 2. 気圧差の平均を計算
                double totalDiff = 0;
                int count = 0;

                for (int i = 1; i < targetHours.Count; i++)
                {
                    double prevPressure = targetHours[i - 1].GetProperty("pressure_mb").GetDouble();
                    double currentPressure = targetHours[i].GetProperty("pressure_mb").GetDouble();

                    // 1時間ごとの変動量（絶対値）を加算
                    totalDiff += Math.Abs(currentPressure - prevPressure);
                    count++;
                }

                // 平均階差（9時から18時までだと9個の差分データができるはず）
                double avgPressureDiff = count > 0 ? Math.Round(totalDiff / count, 2) : 0;

                */

                // 2. 当日の最大気圧差を算出
                double maxDiff = 0;

                for (int i = 1; i < targetHours.Count; i++)
                {
                    double prevPressure = targetHours[i - 1].GetProperty("pressure_mb").GetDouble();
                    double currentPressure = targetHours[i].GetProperty("pressure_mb").GetDouble();
                    // 1時間ごとの変動量（絶対値）を計算
                    double diff = Math.Abs(currentPressure - prevPressure);
                    if (diff > maxDiff)
                    {
                        maxDiff = diff;
                    }
                }

                // 残りのロジックは流用
                int code = current.GetProperty("condition").GetProperty("code").GetInt32();
                int cloud = current.GetProperty("cloud").GetInt32();
                double precip = current.GetProperty("precip_mm").GetDouble();

                // 当日の最高気温 (forecast.day.maxtemp_c)
                double maxTemp = 0;
                if (forecastDay.TryGetProperty("day", out var dayElem) && dayElem.TryGetProperty("maxtemp_c", out var maxtempElem))
                {
                    maxTemp = maxtempElem.GetDouble();
                }

                // 当日のcloud（日中 7:00〜18:00 の hour 配列を対象に平均を計算）
                int dayCloud = 0;
                var dayHours = hours.Where(h =>
                {
                    var timeStr = h.GetProperty("time").GetString();
                    if (DateTime.TryParse(timeStr, out var time))
                    {
                        return time.Hour >= 7 && time.Hour <= 18;
                    }
                    return false;
                }).ToList();

                if (dayHours.Count > 0)
                {
                    var a = dayHours.Select(h => h.GetProperty("cloud").GetInt32());

                    dayCloud = (int)Math.Round(dayHours.Average(h => h.GetProperty("cloud").GetInt32()));
                }

                return new WeatherCondition
                {
                    Description = current.GetProperty("condition").GetProperty("text").GetString() ?? "",
                    Pressure = current.GetProperty("pressure_mb").GetDouble(), // 現在の気圧
                    AvgPressureDiff = maxDiff, // ★算出値
                    WindSpeed = Math.Round((current.GetProperty("wind_kph").GetDouble() / 3.6) * 10) / 10,
                    // WindSpeed = current.GetProperty("wind_kph").GetDouble(),
                    CustomStatus = InterpretWeatherCondition(code, dayCloud, precip, maxTemp)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GASコードをC#へ移植
        private string InterpretWeatherCondition(int code, int dayCloud, double precip, double maxTemp)
        {
            // 1. 【最優先】台風 (雷系)
            int[] stormCodes = { 1087, 1273, 1276, 1279, 1282 };
            if (stormCodes.Contains(code)) return "台風";

            // 2. 【最優先】雪
            int[] snowCodes = { 1066, 1114, 1117, 1210, 1213, 1216, 1219, 1222, 1225, 1255, 1258 };
            if (snowCodes.Contains(code)) return "雪";

            // 2.5 【猛暑判定】その日の最高気温が33度以上かつ当日のcloudが70以下
            if (maxTemp >= 33 && dayCloud <= 70) return "猛暑";

            // 3. 降水量 10mm 以上は「雨」
            if (precip >= 10) return "雨";

            // 4. 雲の量 50% 未満は「晴れ」
            if (dayCloud < 70) return "晴れ";

            // 5. それ以外は「曇り」
            return "曇り";
        }
    }
        
}
