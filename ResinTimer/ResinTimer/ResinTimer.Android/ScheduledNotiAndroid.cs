
using ResinTimer.Droid;

using System;

[assembly: Xamarin.Forms.Dependency(typeof(ScheduledNotiAndroid))]

namespace ResinTimer.Droid
{
    public class ScheduledNotiAndroid : IScheduledNoti
    {
        NotiManager manager;

        public ScheduledNotiAndroid()
        {
            manager = new NotiManager();
        }

        public void CancelAll()
        {
            Cancel<ResinNoti>();
            Cancel<ExpeditionNoti>();
            Cancel<GatheringItemNoti>();
            Cancel<GadgetNoti>();
        }

        public void Cancel<T>() where T : Noti
        {
            var notifier = new NotifierAndroid();

            foreach (var item in manager.GetNotiList<T>())
            {
                notifier.Cancel(item.NotiId);
            }
        }

        public void ScheduleAllNoti()
        {
            Schedule<ResinNoti>();
            Schedule<ExpeditionNoti>();
            Schedule<GatheringItemNoti>();
            Schedule<GadgetNoti>();
        }

        public void Schedule<T>() where T : Noti
        {
            var notifier = new NotifierAndroid();
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

        public void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime)
        {
            var notifier = new NotifierAndroid();

            notifier.Notify(new Notification
            {
                Title = title,
                Text = message,
                Id = id,
                NotifyTime = notiTime
            });
        }

        public void TestNoti(string message = "")
        {
            var notifier = new NotifierAndroid();

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