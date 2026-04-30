using System.Text.Json;
using DailyConditionApp.Models;

namespace DailyConditionApp.Services
{
    public class SettingsService : ISettingService
    {
        // SecureStorageに保存する際の「キー名」を定数として定義
        private const string SecureKey_NotionApi = "NotionApiKey";

        // APIキーを暗号化して保存するメソッド
        public async Task SaveApiKeyAsync(string apiKey)
        {
            // 入力が空の場合は、ストレージから削除する（リセット機能）
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                SecureStorage.Default.Remove(SecureKey_NotionApi);
                return;
            }

            // SecureStorageを使ってOSの安全な領域に保存
            await SecureStorage.Default.SetAsync(SecureKey_NotionApi, apiKey);
        }

        // 保存されたAPIキーを復号してロードするメソッド
        public async Task<string> LoadApiKeyAsync()
        {
            try
            {
                // SecureStorageから読み込み
                string? apiKey = await SecureStorage.Default.GetAsync(SecureKey_NotionApi);

                // nullの場合は空文字を返す
                return apiKey ?? string.Empty;
            }
            catch (Exception)
            {
                // ユーザーが端末のセキュリティ設定を変更した際などに発生する例外のフェイルセーフ
                return string.Empty;
            }
        }
    }
}
