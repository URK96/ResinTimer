using ResinTimer.iOS;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models;
using ResinTimer.Services;

using System;
using System.Collections.Generic;

using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(NotiScheduleiOS))]

namespace ResinTimer.iOS
{
    public class NotiScheduleiOS : NotiScheduleService
    {
        private readonly NotiManager manager;

        public NotiScheduleiOS()
        {
            manager = new NotiManager();
        }

        public override void CancelAll()
        {
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
        }

        public override void Cancel<T>()
        {
            NotifieriOS notifier = new NotifieriOS();
            List<string> cancelList = new List<string>();

            foreach (T item in manager.GetNotiList<T>())
            {
                cancelList.Add(item.NotiId.ToString());
            }

            notifier.Cancel(cancelList.ToArray());
        }

        public override void Schedule<T>()
        {
            NotifieriOS notifier = new NotifieriOS();
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

        public override void TestNoti(string message = "")
        {
            NotifieriOS notifier = new NotifieriOS();

            notifier.Notify(new Notification
            {
                Title = "Test Noti",
                Text = message,
                Id = 990,
                NotifyTime = DateTime.Now.AddSeconds(5)
            }, "TestNoti");
        }

        public override void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime)
        {
            NotifieriOS notifier = new NotifieriOS();

            notifier.Notify(new Notification
            {
                Title = title,
                Text = message,
                Id = id,
                NotifyTime = notiTime
            });
        }
    }
}