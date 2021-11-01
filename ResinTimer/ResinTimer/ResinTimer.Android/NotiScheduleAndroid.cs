﻿using ResinTimer.Droid;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models;
using ResinTimer.Models.Notis;
using ResinTimer.Services;

using System;

[assembly: Xamarin.Forms.Dependency(typeof(NotiScheduleAndroid))]

namespace ResinTimer.Droid
{
    public class NotiScheduleAndroid : NotiScheduleService
    {
        private readonly NotiManager manager;

        public NotiScheduleAndroid()
        {
            manager = new NotiManager();
        }

        public override void Cancel<T>()
        {
            NotifierAndroid notifier = new NotifierAndroid();

            foreach (T item in manager.GetNotiList<T>())
            {
                notifier.Cancel(item.NotiId);
            }
        }

        public static void Cancel(Noti noti)
        {
            NotifierAndroid notifier = new NotifierAndroid();

            notifier.Cancel(noti.NotiId);
        }

        public override void Schedule<T>()
        {
            NotifierAndroid notifier = new NotifierAndroid();
            DateTime now = DateTime.Now;

            foreach (T item in manager.GetNotiList<T>())
            {
                if (item.NotiTime > now)
                {
                    Notification notification = new Notification
                    {
                        Title = item.GetNotiTitle(),
                        Text = item.GetNotiText(),
                        Id = item.NotiId,
                        NotifyTime = item.NotiTime
                    };
                    notification.SetType<T>();

                    notifier.Notify(notification);
                }
            }
        }

        public static void Schedule<T>(Noti noti) where T : Noti
        {
            NotifierAndroid notifier = new NotifierAndroid();
            DateTime now = DateTime.Now;

            if (noti.NotiTime > now)
            {
                Notification notification = new Notification
                {
                    Title = noti.GetNotiTitle(),
                    Text = noti.GetNotiText(),
                    Id = noti.NotiId,
                    NotifyTime = noti.NotiTime
                };
                notification.SetType<T>();

                notifier.Notify(notification);
            }
        }

        public override void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime)
        {
            NotifierAndroid notifier = new NotifierAndroid();

            notifier.Notify(new Notification
            {
                Title = title,
                Text = message,
                Id = id,
                NotifyTime = notiTime
            });
        }

        public override void TestNoti(string message = "")
        {
            NotifierAndroid notifier = new NotifierAndroid();

            notifier.Notify(new Notification
            {
                Title = "Test Noti",
                Text = message,
                Id = 990,
                NotifyTime = DateTime.Now.AddSeconds(5)
            });
        }
    }
}