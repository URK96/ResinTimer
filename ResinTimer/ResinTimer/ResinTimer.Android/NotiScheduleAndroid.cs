using System;

using ResinTimer.Droid;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models;
using ResinTimer.Models.Notis;
using ResinTimer.Services;

[assembly: Xamarin.Forms.Dependency(typeof(NotiScheduleAndroid))]

namespace ResinTimer.Droid
{
    public class NotiScheduleAndroid : NotiScheduleService
    {
        private readonly NotiManager manager;

        public NotiScheduleAndroid()
        {
            manager = new();
        }

        public override void Cancel<T>()
        {
            NotifierAndroid notifier = new();

            foreach (T item in manager.GetNotiList<T>())
            {
                notifier.Cancel(item.NotiId);
            }
        }

        public static void Cancel(Noti noti)
        {
            NotifierAndroid notifier = new();

            notifier.Cancel(noti.NotiId);
        }

        public override void Schedule<T>()
        {
            NotifierAndroid notifier = new();
            DateTime now = DateTime.Now;

            foreach (T item in manager.GetNotiList<T>())
            {
                if (item.NotiTime > now)
                {
                    Notification notification = new()
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
            NotifierAndroid notifier = new();
            DateTime now = DateTime.Now;

            if (noti.NotiTime > now)
            {
                Notification notification = new()
                {
                    Title = noti.GetNotiTitle(),
                    Text = noti.GetNotiText(),
                    Id = noti.NotiId,
                    IconData = noti.GetIconData(),
                    NotifyTime = noti.NotiTime
                };
                notification.SetType<T>();

                notifier.Notify(notification);
            }
        }

        public override void ScheduleNotiItem<T>(Noti noti)
        {
            NotifierAndroid notifier = new();
            DateTime now = DateTime.Now;

            if (noti.NotiTime > now)
            {
                Notification notification = new()
                {
                    Title = noti.GetNotiTitle(),
                    Text = noti.GetNotiText(),
                    Id = noti.NotiId,
                    IconData = noti.GetIconData(),
                    NotifyTime = noti.NotiTime
                };
                notification.SetType<T>();

                notifier.Notify(notification);
            }
        }

        public override void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime)
        {
            NotifierAndroid notifier = new();

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
            NotifierAndroid notifier = new();

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