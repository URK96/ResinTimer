using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models;
using ResinTimer.Services;
using ResinTimer.UWP;

using System;

[assembly: Xamarin.Forms.Dependency(typeof(NotiScheduleUWP))]

namespace ResinTimer.UWP
{
    public class NotiScheduleUWP : NotiScheduleService
    {
        private readonly NotiManager manager;

        public NotiScheduleUWP()
        {
            manager = new NotiManager();
        }

        public override void Cancel<T>()
        {
            NotifierUWP notifier = new NotifierUWP();

            foreach (T item in manager.GetNotiList<T>())
            {
                notifier.Cancel(item.NotiId.ToString());
            }
        }

        public override void Schedule<T>()
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

        public override void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime)
        {
            
        }

        public override void TestNoti(string message = "")
        {
            
        }

        public override void CancelAll()
        {
            NotifierUWP notifier = new NotifierUWP();

            notifier.CancelAll();
        }
    }
}
