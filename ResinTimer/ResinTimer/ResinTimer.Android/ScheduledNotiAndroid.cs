using Android.App;

using Newtonsoft.Json;

using ResinTimer.Droid;

using System;
using System.Collections.Generic;

using Xamarin.Essentials;

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
        }

        public void Schedule<T>() where T : Noti
        {
            var notifier = new NotifierAndroid();
            var now = DateTime.Now;

            foreach (var item in manager.GetNotiList<T>())
            {
                if (item.NotiTime > now)
                {
                    notifier.Notify(new Notification
                    {
                        Title = item.GetNotiTitle(),
                        Text = item.GetNotiText(),
                        Id = item.NotiId,
                        NotifyTime = item.NotiTime
                    });
                }
            }
        }

        public void TestNoti()
        {

        }
    }
}