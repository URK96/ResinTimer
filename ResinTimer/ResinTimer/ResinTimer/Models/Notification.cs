using ResinTimer.Models.Notis;

using System;

using NotificationType = ResinTimer.Managers.NotiManagers.NotiManager.NotificationType;

namespace ResinTimer.Models
{
    public class Notification
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// Notification identifier used for canceling not scheduled notification
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the notification title.
        /// </summary>
        /// <value>
        /// The notification title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the notification text.
        /// </summary>
        /// <value>
        /// The notification text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the notify time of notification.
        /// </summary>
        /// <value>
        /// The notify time of notification.
        /// </value>
        public DateTime NotifyTime { get; set; }

        public NotificationType NotiType { get; set; }

        public byte[] IconData { get; set; }

        public void SetType<T>() where T : Noti
        {
            Type t = typeof(T);

            NotiType = t switch
            {
                _ when t.IsAssignableFrom(typeof(ResinNoti)) => NotificationType.Resin,
                _ when t.IsAssignableFrom(typeof(RealmCurrencyNoti)) => NotificationType.RealmCurrency,
                _ when t.IsAssignableFrom(typeof(RealmFriendshipNoti)) => NotificationType.RealmFriendship,
                _ when t.IsAssignableFrom(typeof(ExpeditionNoti)) => NotificationType.Expedition,
                _ when t.IsAssignableFrom(typeof(GatheringItemNoti)) => NotificationType.GatheringItem,
                _ when t.IsAssignableFrom(typeof(GadgetNoti)) => NotificationType.Gadget,
                _ when t.IsAssignableFrom(typeof(FurnishingNoti)) => NotificationType.Furnishing,
                _ when t.IsAssignableFrom(typeof(GardeningNoti)) => NotificationType.Gardening,
                _ => NotificationType.Resin
            };
        }
    }
}
