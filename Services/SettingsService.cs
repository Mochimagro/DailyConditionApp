using System.Text.Json;
using DailyConditionApp.Models;

namespace DailyConditionApp.Services
{
    public class SettingsService : ISettingsService
    {
        private const string SecureKey_NotionToken = "NotionApiToken";
        private const string SecureKey_DatabaseId = "NotionDatabaseId";
        private const string SecureKey_WeatherApi = "WeatherApiKey";
        private const string SecureKey_Lat = "Latitude";
        private const string SecureKey_Lon = "Longitude";

        // APIキーを暗号化して保存するメソッド
        public async Task SaveNotionSettingsAsync(string token,string databeseID)
        {
            if (string.IsNullOrWhiteSpace(token)) SecureStorage.Default.Remove(SecureKey_DatabaseId);
            else await SecureStorage.Default.SetAsync(SecureKey_DatabaseId, databeseID);

            if (string.IsNullOrWhiteSpace(token)) SecureStorage.Default.Remove(SecureKey_NotionToken);
            else await SecureStorage.Default.SetAsync(SecureKey_NotionToken, token);
        }

        // 保存されたAPIキーを復号してロードするメソッド
        public async Task<(string token, string databaseId)> LoadNotionKeyAsync()
        {
            try
            {
                // SecureStorageから読み込み
                string? token = await SecureStorage.Default.GetAsync(SecureKey_NotionToken);

                string? databaseId = await SecureStorage.Default.GetAsync(SecureKey_DatabaseId);

                // nullの場合は空文字を返す
                return (token ?? string.Empty, databaseId ?? string.Empty);
            }
            catch (Exception)
            {
                // ユーザーが端末のセキュリティ設定を変更した際などに発生する例外のフェイルセーフ
                return (string.Empty, string.Empty);
            }
        }
        // --- WeatherAPI用 (新規追加) ---
        public async Task SaveWeatherSettingsAsync(string apiKey, string lat, string lon)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) SecureStorage.Default.Remove(SecureKey_WeatherApi);
            else await SecureStorage.Default.SetAsync(SecureKey_WeatherApi, apiKey);

            if (string.IsNullOrWhiteSpace(lat)) SecureStorage.Default.Remove(SecureKey_Lat);
            else await SecureStorage.Default.SetAsync(SecureKey_Lat, lat);

            if (string.IsNullOrWhiteSpace(lon)) SecureStorage.Default.Remove(SecureKey_Lon);
            else await SecureStorage.Default.SetAsync(SecureKey_Lon, lon);
        }

        public async Task<(string ApiKey, string Lat, string Lon)> LoadWeatherSettingsAsync()
        {
            try
            {
                string apiKey = await SecureStorage.Default.GetAsync(SecureKey_WeatherApi) ?? string.Empty;
                string lat = await SecureStorage.Default.GetAsync(SecureKey_Lat) ?? string.Empty;
                string lon = await SecureStorage.Default.GetAsync(SecureKey_Lon) ?? string.Empty;
                return (apiKey, lat, lon);
            }
            catch (Exception)
            {
                return (string.Empty, string.Empty, string.Empty);
            }
        }
    }
}
