﻿using Android.App;
using Android.Content;
using Android.Content.PM;

using AndroidX.Core.App;

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
            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = DeserializeNotification(extra);

            var nativeNotification = CreateNativeNotification(context, notification);

            NotificationManager.Notify(notification.Id, nativeNotification);
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
                
            if (context.PackageManager.GetLaunchIntentForPackage("com.miHoYo.GenshinImpact") != null)
            {
                var runIntent = new Intent(context, typeof(NotiActionReceiver));
                runIntent.SetAction("RUN_GENSHIN");
                runIntent.PutExtra("NotiId", notification.Id);

                var pRunIntent = PendingIntent.GetBroadcast(context, 0, runIntent, PendingIntentFlags.UpdateCurrent);

                builder.AddAction(0, AppResources.Noti_QuickAction_RunGenshinApp, pRunIntent);
            }

            if ((notification.NotiType == NotiManager.NotiType.Expedition) ||
                (notification.NotiType == NotiManager.NotiType.GatheringItem) ||
                (notification.NotiType == NotiManager.NotiType.Gadget))
            {
                var resetIntent = new Intent(context, typeof(NotiActionReceiver));
                resetIntent.SetAction("RESET_TIMER");
                resetIntent.PutExtra("NotiId", notification.Id);
                resetIntent.PutExtra("NotiType", (int)notification.NotiType);

                var pResetIntent = PendingIntent.GetBroadcast(context, 0, resetIntent, PendingIntentFlags.UpdateCurrent);

                builder.AddAction(0, AppResources.Noti_QuickAction_ResetTimer, pResetIntent);
            }

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

        [BroadcastReceiver]
        [IntentFilter(new string[] { "RUN_GENSHIN", "RESET_TIMER" })]
        public class NotiActionReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                switch (intent.Action)
                {
                    case "RUN_GENSHIN":
                        var rIntent = context.PackageManager.GetLaunchIntentForPackage("com.miHoYo.GenshinImpact");
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
                var id = intent.GetIntExtra("NotiId", -1);
                var type = (NotiManager.NotiType)intent.GetIntExtra("NotiType", 0);
                NotiManager notiManager = type switch
                {
                    NotiManager.NotiType.Expedition => new ExpeditionNotiManager(),
                    NotiManager.NotiType.GatheringItem => new GatheringItemNotiManager(),
                    _ => null
                };

                if ((notiManager == null) ||
                    (id == -1))
                {
                    return;
                }

                var noti = notiManager.Notis.Find(x => x.NotiId.Equals(id));

                ScheduledNotiAndroid.Cancel(noti);
                noti.UpdateTime();

                notiManager.SaveNotis();
                
                switch (type)
                {
                    case NotiManager.NotiType.Expedition:
                        //notiManager.UpdateScheduledNoti<ExpeditionNoti>();
                        ScheduledNotiAndroid.Schedule<ExpeditionNoti>(noti);
                        break;
                    case NotiManager.NotiType.GatheringItem:
                        //notiManager.UpdateScheduledNoti<GatheringItemNoti>();
                        ScheduledNotiAndroid.Schedule<GatheringItemNoti>(noti);
                        break;
                    case NotiManager.NotiType.Gadget:
                        //notiManager.UpdateScheduledNoti<GadgetNoti>();
                        ScheduledNotiAndroid.Schedule<GadgetNoti>(noti);
                        break;
                    default:
                        break;
                }


            }
        }
    }
}