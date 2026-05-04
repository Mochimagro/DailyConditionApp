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
    double SleepTime,
    double SleepEfficiency,
    string WeatherLabel,
    double PressureDiff,
    double WindSpeed,
    string? RelatedPageId = null // 必要に応じてリレーション用
);
    public record TodayConditionResult(double EnvironmentScore, string ConditionComment);

    public class NotionService: INotionService
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
                ["睡眠時間(h)"] = new { number = logData.SleepTime },
                ["睡眠効率"] = new { number = logData.SleepEfficiency },
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

        private const string NotionQueryUrl = "https://api.notion.com/v1/databases/{0}/query";

        public async Task<TodayConditionResult?> GetTodayConditionAsync(string token, string databaseId, string dateString)
        {
            string url = string.Format(NotionQueryUrl, databaseId);

            // 1. クエリのフィルター条件（日付プロパティが「今日」と一致するもの）
            // ※プロパティ名「日付」はご自身のNotionに合わせて変更してください
            var payload = new
            {
                filter = new
                {
                    property = "日付",
                    date = new { @equals = dateString }
                }
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Notion-Version", NotionVersion);
            request.Content = content;

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                string jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                var results = doc.RootElement.GetProperty("results");

                if (results.GetArrayLength() == 0) return null; // 今日のデータがまだ無い場合

                // 2. 最新の1件目を取得
                var properties = results[0].GetProperty("properties");

                // 3. 値の抽出
                // ※Notionのプロパティ型（number, rich_text, selectなど）に合わせてパースします
                var score = ExtractNotionNumber(properties, "環境コンディションスコア");
                string comment = ExtractNotionRichText(properties, "コンディション一言評価");

                return new TodayConditionResult(score, comment);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // --- JSON抽出用のヘルパーメソッド ---

        private double ExtractNotionNumber(JsonElement properties, string propertyName)
        {
            if (properties.TryGetProperty(propertyName, out var prop) && prop.TryGetProperty("formula", out var formula) && formula.TryGetProperty("number",out var number))
            {
                return number.ValueKind != JsonValueKind.Null ? number.GetDouble() : 0;
            }
            return 0;
        }

        private string ExtractNotionRichText(JsonElement properties, string propertyName)
        {
            if (properties.TryGetProperty(propertyName, out var prop) && prop.TryGetProperty("formula",out var formula) && formula.TryGetProperty("string", out var richTextArray))
            {
                return richTextArray.ValueKind != JsonValueKind.Null ? richTextArray.ToString() : "";
            }
            return "データなし";
        }
    }
}
