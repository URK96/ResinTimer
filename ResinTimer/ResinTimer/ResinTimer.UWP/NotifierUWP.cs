using Microsoft.Toolkit.Uwp.Notifications;

using System.Collections.Generic;
using System.Linq;

using Windows.UI.Notifications;

namespace ResinTimer.UWP
{
    public class NotifierUWP
    {
        public ToastNotifier UWPNotifier => UWPAppEnvironment.toastNotifier;

        public void Notify(Notification notification)
        {
            ToastContent builder = new ToastContentBuilder()
                .AddToastActivationInfo("ResinNoti", ToastActivationType.Foreground)
                .AddText(notification.Title)
                .AddText(notification.Text)
                .GetToastContent();

            var toast = new ScheduledToastNotification(builder.GetXml(), notification.NotifyTime)
            {
                Tag = notification.Id.ToString(),
                Group = $"ResinNoti"
            };

            UWPNotifier.AddToSchedule(toast);
        }

        public void Cancel(string tag)
        {
            var scheduledList = GetScheduledList();

            var toastItem = scheduledList.FirstOrDefault(x => x.Tag.Equals(tag));

            if (toastItem != null)
            {
                UWPNotifier.RemoveFromSchedule(toastItem);
            }
        }

        public void CancelAll()
        {
            foreach (var item in GetScheduledList())
            {
                UWPNotifier.RemoveFromSchedule(item);
            }
        }

        public IReadOnlyList<ScheduledToastNotification> GetScheduledList() => UWPNotifier.GetScheduledToastNotifications();
    }
}
