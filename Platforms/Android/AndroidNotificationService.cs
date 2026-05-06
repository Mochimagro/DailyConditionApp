#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using DailyConditionApp.Services;

[assembly: Dependency(typeof(DailyConditionApp.Platforms.Android.AndroidNotificationService))]
namespace DailyConditionApp.Platforms.Android
{

    public class AndroidNotificationService : INotificationService
    {
        const string ChannelId = "condition_channel";
        const int NotificationId = 1001;

        public void ShowPersistentNotification(string title, string message)
        {
            var context = Platform.AppContext;
            var manager = context.GetSystemService(Context.NotificationService) as NotificationManager;

            // Android 8.0以上はチャンネルが必要
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "Condition Score", NotificationImportance.Low)
                {
                    Description = "Displays the daily condition score"
                };
                manager?.CreateNotificationChannel(channel);
            }

            // 通知をクリックしたときにアプリを開く設定
            var intent = context.PackageManager?.GetLaunchIntentForPackage(context.PackageName);
            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);

            var builder = new NotificationCompat.Builder(context, ChannelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Mipmap.appicon) // アプリアイコンを指定
                .SetOngoing(true) // ★消えない設定（常駐）
                .SetPriority(NotificationCompat.PriorityLow)
                .SetContentIntent(pendingIntent);

            manager?.Notify(NotificationId, builder.Build());
        }
    }
}
#endif