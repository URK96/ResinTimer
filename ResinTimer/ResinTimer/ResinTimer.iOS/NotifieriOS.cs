using Foundation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;

using UserNotifications;

namespace ResinTimer.iOS
{
    public class NotifieriOS
    {
        public async void Notify(Notification notification, string id = "")
        {
            var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();

            if (settings.AlertSetting != UNNotificationSetting.Enabled)
            {
                var toast = new ToastiOS();
                toast.Show("Check Noti Permission");
            }

            var noti = new UNMutableNotificationContent
            {
                Title = notification.Title,
                Subtitle = string.Empty,
                Body = notification.Text,
                Badge = 0
            };

            var dt = notification.NotifyTime;

            var dateComponent = new NSDateComponents
            {
                Calendar = NSCalendar.CurrentCalendar,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                Hour = dt.Hour,
                Minute = dt.Minute,
                Second = dt.Second
            };

            var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponent, false);
            var request = UNNotificationRequest.FromIdentifier(string.IsNullOrWhiteSpace(id) ? notification.Id.ToString() : id, noti, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    var toast = new ToastiOS();

                    toast.Show("Ooops, error");
                }
            });
        }

        public void Cancel(string[] cancelList)
        {
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(cancelList);
        }

        public void CancelAll()
        {
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
        }
    }
}