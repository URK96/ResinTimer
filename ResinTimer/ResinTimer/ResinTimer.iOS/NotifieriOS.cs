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
        public void Notify(Notification notification)
        {
            var noti = new UNMutableNotificationContent
            {
                Title = notification.Title,
                Subtitle = notification.Title,
                Body = notification.Text,
                Badge = 1
            };
        }

        public void Cancel(string tag)
        {
            //var scheduledList = GetScheduledList();

            //var toastItem = scheduledList.FirstOrDefault(x => x.Tag.Equals(tag));

            //UWPNotifier.RemoveFromSchedule(toastItem);
        }

        public void CancelAll()
        {
            //foreach (var item in GetScheduledList())
            //{
            //    UWPNotifier.RemoveFromSchedule(item);
            //}
        }

        //public IReadOnlyList<ScheduledToastNotification> GetScheduledList() => UWPNotifier.GetScheduledToastNotifications();
    }
}