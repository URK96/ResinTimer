using Android.App;
using Android.Content;

using AndroidX.Core.App;

using System.IO;
using System.Xml.Serialization;

namespace ResinTimer.Droid
{
    [BroadcastReceiver]
    public class ScheduledAlarmReceiver : BroadcastReceiver
    {
        public const string LocalNotificationKey = "LocalNotification";

        public NotificationManager NotiManager => Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        public override void OnReceive(Context context, Intent intent)
        {
            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = DeserializeNotification(extra);

            var nativeNotification = CreateNativeNotification(context, notification);

            NotiManager.Notify(notification.Id, nativeNotification);
        }

        private Android.App.Notification CreateNativeNotification(Context context, Notification notification)
        {
            //var intent = new Intent(context, typeof(SplashActivity));
            //var stackBuilder = AndroidX.Core.App.TaskStackBuilder.Create(context);
            //stackBuilder.AddParentStack(new SplashActivity());
            //stackBuilder.AddNextIntent(intent);

            var builder = new NotificationCompat.Builder(Application.Context, AndroidAppEnvironment.CHANNEL_ID)
                .SetAutoCancel(true)
                .SetVisibility((int)NotificationVisibility.Public)
                .SetContentIntent(PendingIntent.GetActivity(context, 0, new Intent(context, typeof(SplashActivity)), PendingIntentFlags.UpdateCurrent))
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Text)
                .SetSmallIcon(Application.Context.ApplicationInfo.Icon);

            var nativeNotification = builder.Build();

            return nativeNotification;
        }

        private Notification DeserializeNotification(string notificationString)
        {
            var xmlSerializer = new XmlSerializer(typeof(Notification));
            using var stringReader = new StringReader(notificationString);

            var notification = xmlSerializer.Deserialize(stringReader) as Notification;

            return notification;
        }
    }
}