using Android.App;
using Android.Content;

using AndroidX.Core.App;

using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System.IO;
using System.Xml.Serialization;

namespace ResinTimer.Droid
{
    [BroadcastReceiver]
    public class ScheduledAlarmReceiver : BroadcastReceiver
    {
        public const string LocalNotificationKey = "LocalNotification";

        public NotificationManager NotificationManager => Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        public override void OnReceive(Context context, Intent intent)
        {
            string extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = DeserializeNotification(extra);

            Android.App.Notification nativeNotification = CreateNativeNotification(context, notification);

            NotificationManager.Notify(notification.Id, nativeNotification);
        }

        private Android.App.Notification CreateNativeNotification(Context context, Models.Notification notification)
        {
            var builder = new NotificationCompat.Builder(Application.Context, AndroidAppEnvironment.CHANNEL_ID)
                .SetAutoCancel(true)
                .SetVisibility((int)NotificationVisibility.Public)
                .SetContentIntent(PendingIntent.GetActivity(context, 0, new Intent(context, typeof(SplashActivity)), PendingIntentFlags.UpdateCurrent))
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Text)
                .SetSmallIcon(Application.Context.ApplicationInfo.Icon);

            if (context.PackageManager.GetLaunchIntentForPackage("com.miHoYo.GenshinImpact") != null)
            {
                Intent runIntent = new Intent(context, typeof(NotiActionReceiver))
                    .SetAction("RUN_GENSHIN")
                    .PutExtra("NotiId", notification.Id);

                PendingIntent pRunIntent = PendingIntent.GetBroadcast(context, 0, runIntent, PendingIntentFlags.UpdateCurrent);

                builder.AddAction(0, AppResources.Noti_QuickAction_RunGenshinApp, pRunIntent);
            }

            if (!((notification.NotiType == NotiManager.NotiType.Resin) ||
                (notification.NotiType == NotiManager.NotiType.RealmCurrency) ||
                (notification.NotiType == NotiManager.NotiType.RealmFriendship)))
            {
                Intent resetIntent = new Intent(context, typeof(NotiActionReceiver))
                    .SetAction("RESET_TIMER")
                    .PutExtra("NotiId", notification.Id)
                    .PutExtra("NotiType", (int)notification.NotiType);

                PendingIntent pResetIntent = PendingIntent.GetBroadcast(context, 0, resetIntent, PendingIntentFlags.UpdateCurrent);

                builder.AddAction(0, AppResources.Noti_QuickAction_ResetTimer, pResetIntent);
            }

            Android.App.Notification nativeNotification = builder.Build();

            return nativeNotification;
        }

        private Models.Notification DeserializeNotification(string notificationString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Models.Notification));
            using StringReader stringReader = new StringReader(notificationString);

            var notification = xmlSerializer.Deserialize(stringReader) as Models.Notification;

            return notification;
        }

        [BroadcastReceiver]
        [IntentFilter(new string[] { "RUN_GENSHIN", "RESET_TIMER" })]
        public class NotiActionReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                switch (intent.Action)
                {
                    case "RUN_GENSHIN":
                        Intent rIntent = context.PackageManager.GetLaunchIntentForPackage("com.miHoYo.GenshinImpact");

                        context.StartActivity(rIntent);

                        (context.GetSystemService(Context.NotificationService) as NotificationManager).Cancel(intent.GetIntExtra("NotiId", -1));
                        break;
                    case "RESET_TIMER":
                        ResetTimer(intent);
                        break;
                }
            }

            private void ResetTimer(Intent intent)
            {
                int id = intent.GetIntExtra("NotiId", -1);
                NotiManager.NotiType type = (NotiManager.NotiType)intent.GetIntExtra("NotiType", 0);
                NotiManager notiManager = type switch
                {
                    NotiManager.NotiType.Expedition => new ExpeditionNotiManager(),
                    NotiManager.NotiType.GatheringItem => new GatheringItemNotiManager(),
                    NotiManager.NotiType.Gadget => new GadgetNotiManager(),
                    NotiManager.NotiType.Furnishing => new FurnishingNotiManager(),
                    NotiManager.NotiType.Gardening => new GardeningNotiManager(),
                    _ => null
                };

                if ((notiManager == null) ||
                    (id == -1))
                {
                    return;
                }

                Noti noti = notiManager.Notis.Find(x => x.NotiId.Equals(id));

                NotiScheduleAndroid.Cancel(noti);

                noti.UpdateTime();
                notiManager.SaveNotis();
                
                switch (type)
                {
                    case NotiManager.NotiType.Expedition:
                        NotiScheduleAndroid.Schedule<ExpeditionNoti>(noti);
                        break;
                    case NotiManager.NotiType.GatheringItem:
                        NotiScheduleAndroid.Schedule<GatheringItemNoti>(noti);
                        break;
                    case NotiManager.NotiType.Gadget:
                        NotiScheduleAndroid.Schedule<GadgetNoti>(noti);
                        break;
                    case NotiManager.NotiType.Furnishing:
                        NotiScheduleAndroid.Schedule<FurnishingNoti>(noti);
                        break;
                    case NotiManager.NotiType.Gardening:
                        NotiScheduleAndroid.Schedule<GardeningNoti>(noti);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}