using Foundation;

using Newtonsoft.Json;

using ResinTimer.iOS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;

using UserNotifications;

using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(ScheduledNotiiOS))]

namespace ResinTimer.iOS
{
    public class ScheduledNotiiOS : IScheduledNoti
    {
        public void CancelAll()
        {
            var notifier = new NotifieriOS();

            notifier.CancelAll();
        }

        public void Cancel<T>() where T : Noti
        {
            var notifier = new NotifieriOS();

            //notifier.Cancel()
        }

        public void ScheduleAllNoti()
        {
            var list = JsonConvert.DeserializeObject<List<ResinNoti>>(Preferences.Get(SettingConstants.NOTI_LIST, string.Empty));
            var title = Resources.AppResources.NotiTitle;
            var text = Resources.AppResources.NotiText;
            var notifier = new NotifieriOS();
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

        public void Schedule<T>() where T : Noti
        {
            throw new NotImplementedException();
        }

        public async void TestNoti()
        {
            var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();

            if (settings.AlertSetting != UNNotificationSetting.Enabled)
            {
                var toast = new ToastiOS();
                toast.Show("No Noti permission");
            }

            var content = new UNMutableNotificationContent
            {
                Title = "Test Noti",
                Subtitle = "Resin test noti",
                Body = "This is resin test noti",
                Badge = 1
            };

            var now = DateTime.Now.AddSeconds(5);

            var dateComponent = new NSDateComponents()
            {
                Month = now.Month,
                Day = now.Day,
                Year = now.Year,
                Hour = now.Hour,
                Minute = now.Minute,
                Second = now.Second
            };

            var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponent, false);

            var requestID = "testNoti";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => 
            {
                if (err != null)
                {
                    var toast = new ToastiOS();
                    toast.Show("Ooops, error");
                }
            });
        }

        public List<T> GetNotiList<T>() where T : Noti
        {
            throw new NotImplementedException();
        }

        public void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime)
        {
            throw new NotImplementedException();
        }

        public void TestNoti(string message = "")
        {
            throw new NotImplementedException();
        }
    }
}