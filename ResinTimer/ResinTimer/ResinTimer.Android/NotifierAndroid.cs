﻿using Android.App;
using Android.Content;
using Android.OS;

using AndroidX.Core.App;

using Java.Util;

using System;
using System.IO;
using System.Xml.Serialization;

namespace ResinTimer.Droid
{
    public class NotifierAndroid
    {
        public AlarmManager AlarmManager => Application.Context.GetSystemService(NotificationCompat.CategoryAlarm) as AlarmManager;
        public NotificationManager NotiManager => Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        /// <summary>
        /// Notifies the specified notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void Notify(Notification notification)
        {
            var triggerTime = NotifyTimeInMilliseconds(notification.NotifyTime);

            AlarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + triggerTime, CreatePendingIntent(notification.Id, notification));
        }

        /// <summary>
        /// Cancels the specified notification identifier.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        public void Cancel(int notificationId)
        {
            AlarmManager.Cancel(CreatePendingIntent(notificationId));
            NotiManager.Cancel(notificationId);
        }

        private PendingIntent CreatePendingIntent(int id, Notification noti = null)
        {
            var intent = new Intent(Application.Context, typeof(ScheduledAlarmReceiver)).SetAction($"LocalNotiIntent{id}");

            if (noti != null)
            {
                intent.PutExtra(ScheduledAlarmReceiver.LocalNotificationKey, SerializeNotification(noti));
            }

            return PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        private string SerializeNotification(Notification notification)
        {
            var xmlSerializer = new XmlSerializer(notification.GetType());
            using var stringWriter = new StringWriter();

            xmlSerializer.Serialize(stringWriter, notification);

            return stringWriter.ToString();
        }

        private long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcAlarmTimeInMillis = (notifyTime.ToUniversalTime() - DateTime.UtcNow).TotalMilliseconds;

            return (long)utcAlarmTimeInMillis;
        }
    }
}