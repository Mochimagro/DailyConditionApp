using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public record DailyLogData(
    string Date, // "yyyy-MM-dd"
    string WeatherLabel,
    double PressureDiff,
    double WindSpeed,
    string? RelatedPageId = null // 必要に応じてリレーション用
);
    class NotionService: INotionService
    {
        private readonly HttpClient _httpClient;
        private const string NotionVersion = "2022-06-28";
        private const string NotionApiUrl = "https://api.notion.com/v1/pages";

        public NotionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> AddDailyLogAsync(string token, string databaseId, DailyLogData logData)
        {
            // 1. Notion API用のプロパティ（JSONの"properties"の中身）をDictionaryで構築
            var properties = new Dictionary<string, object>
            {
                ["名前"] = new { title = new[] { new { text = new { content = $"{logData.Date} のログ" } } } },
                ["日付"] = new { date = new { start = logData.Date } },
                ["天気"] = new { select = new { name = logData.WeatherLabel } },
                ["当日気圧差"] = new { number = logData.PressureDiff },
                ["風速(m/s)"] = new { number = logData.WindSpeed }
            };

            // リレーション（今日の俺ノートDB）のIDが渡されていれば追加
            if (!string.IsNullOrEmpty(logData.RelatedPageId))
            {
                properties["今日の俺ノートDB"] = new { relation = new[] { new { id = logData.RelatedPageId } } };
            }

            // 2. 送信用の最終的なペイロードを作成
            var payload = new
            {
                parent = new { database_id = databaseId },
                properties = properties
            };

            // 3. JSON文字列に変換
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 4. リクエストの作成とヘッダーの付与
            using var request = new HttpRequestMessage(HttpMethod.Post, NotionApiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Notion-Version", NotionVersion);
            request.Content = content;

            try
            {
                // 5. 送信
                var response = await _httpClient.SendAsync(request);

                // 成功したかどうかを返す（ログ出しが必要なら response.Content.ReadAsStringAsync() を使用）
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                // 通信エラー時
                return false;
            }
        }
    }
}
