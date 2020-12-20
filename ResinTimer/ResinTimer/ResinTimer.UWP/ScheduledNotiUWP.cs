using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ResinTimer.UWP;
using ResinTimer;

using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(ScheduledNotiUWP))]

namespace ResinTimer.UWP
{
    public class ScheduledNotiUWP : IScheduledNoti
    {
        public void CancelAll()
        {
            var notifier = new NotifierUWP();

            notifier.CancelAll();
        }

        public void ScheduleAllNoti()
        {
            var list = JsonConvert.DeserializeObject<List<Noti>>(Preferences.Get(SettingConstants.NOTI_LIST, string.Empty));
            var title = Resources.AppResources.NotiTitle;
            var text = Resources.AppResources.NotiText;
            var notifier = new NotifierUWP();
            var now = DateTime.Now;

            foreach (var item in list)
            {
                if (item.NotiTime > now)
                {
                    notifier.Notify(new Notification
                    {
                        Title = title,
                        Text = $"{item.Resin} {text}",
                        Id = item.NotiId,
                        NotifyTime = item.NotiTime
                    });
                }
            }
        }
    }
}
