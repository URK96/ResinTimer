using ResinTimer.iOS;

using System;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(ScheduledNotiiOS))]

namespace ResinTimer.iOS
{
    public class ScheduledNotiiOS : IScheduledNoti
    {
        NotiManager manager;

        public ScheduledNotiiOS()
        {
            manager = new NotiManager();
        }

        public void CancelAll()
        {
            var notifier = new NotifieriOS();

            notifier.CancelAll();
        }

        public void Cancel<T>() where T : Noti
        {
            var notifier = new NotifieriOS();
            var cancelList = new List<string>();

            foreach (var item in manager.GetNotiList<T>())
            {
                cancelList.Add(item.NotiId.ToString());
            }

            notifier.Cancel(cancelList.ToArray());
        }

        public void ScheduleAllNoti()
        {
            Schedule<ResinNoti>();
            Schedule<RealmCurrencyNoti>();
            Schedule<RealmFriendshipNoti>();
            Schedule<ExpeditionNoti>();
            Schedule<GatheringItemNoti>();
            Schedule<GadgetNoti>();
            Schedule<FurnishingNoti>();
        }

        public void Schedule<T>() where T : Noti
        {
            var notifier = new NotifieriOS();
            var now = DateTime.Now;

            foreach (var item in manager.GetNotiList<T>())
            {
                if (item.NotiTime > now)
                {
                    var notification = new Notification
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

        public void TestNoti(string message = "")
        {
            var notifier = new NotifieriOS();

            notifier.Notify(new Notification
            {
                Title = "Test Noti",
                Text = message,
                Id = 990,
                NotifyTime = DateTime.Now.AddSeconds(5)
            }, "TestNoti");
        }

        public void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime)
        {
            var notifier = new NotifieriOS();

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