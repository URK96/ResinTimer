using ResinTimer.UWP;

using System;

[assembly: Xamarin.Forms.Dependency(typeof(ScheduledNotiUWP))]

namespace ResinTimer.UWP
{
    public class ScheduledNotiUWP : IScheduledNoti
    {
        NotiManager manager;

        public ScheduledNotiUWP()
        {
            manager = new NotiManager();
        }

        public void CancelAll()
        {
            NotifierUWP notifier = new NotifierUWP();

            notifier.CancelAll();
        }

        public void Cancel<T>() where T : Noti
        {
            NotifierUWP notifier = new NotifierUWP();

            foreach (T item in manager.GetNotiList<T>())
            {
                notifier.Cancel(item.NotiId.ToString());
            }
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
            Schedule<GardeningNoti>();
        }

        public void Schedule<T>() where T : Noti
        {
            NotifierUWP notifier = new NotifierUWP();
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

        public void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime)
        {
            throw new NotImplementedException();
        }

        public void TestNoti(string message = "")
        {
            
        }
    }
}
