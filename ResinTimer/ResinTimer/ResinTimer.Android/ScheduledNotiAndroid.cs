using Android.App;

using Newtonsoft.Json;

using ResinTimer.Droid;

using System;
using System.Collections.Generic;

using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(ToastAndroid))]

namespace ResinTimer.Droid
{
    public class ScheduledNotiAndroid : IScheduledNoti
    {
        public void CancelAll()
        {
            var list = JsonConvert.DeserializeObject<List<Noti>>(Preferences.Get(SettingConstants.NOTI_LIST, string.Empty));
            var notifier = new NotifierAndroid();

            foreach (var item in list)
            {
                notifier.Cancel(item.NotiId);
            }
        }

        public void ScheduleAllNoti()
        {
            var list = JsonConvert.DeserializeObject<List<Noti>>(Preferences.Get(SettingConstants.NOTI_LIST, string.Empty));
            var title = Application.Context.Resources.GetString(Resource.String.NotiTitle);
            var text = Application.Context.Resources.GetString(Resource.String.NotiText);
            var notifier = new NotifierAndroid();
            var now = DateTime.Now;

            foreach (var item in list)
            {
                if (item.NotiTime > now)
                {
                    notifier.Notify(new Notification
                    {
                        Title = title,
                        Text = $"{item.Resin}{text}",
                        Id = item.NotiId,
                        NotifyTime = item.NotiTime
                    });
                }
            }
        }
    }
}