using ResinTimer.Models.Notis;

using System;

using NotiType = ResinTimer.Managers.NotiManagers.NotiManager.NotiType;

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

        public NotiType NotiType { get; set; }

        public void SetType<T>() where T : Noti
        {
            Type t = typeof(T);

            NotiType = t switch
            {
                _ when t.IsAssignableFrom(typeof(ResinNoti)) => NotiType.Resin,
                _ when t.IsAssignableFrom(typeof(RealmCurrencyNoti)) => NotiType.RealmCurrency,
                _ when t.IsAssignableFrom(typeof(RealmFriendshipNoti)) => NotiType.RealmFriendship,
                _ when t.IsAssignableFrom(typeof(ExpeditionNoti)) => NotiType.Expedition,
                _ when t.IsAssignableFrom(typeof(GatheringItemNoti)) => NotiType.GatheringItem,
                _ when t.IsAssignableFrom(typeof(GadgetNoti)) => NotiType.Gadget,
                _ when t.IsAssignableFrom(typeof(FurnishingNoti)) => NotiType.Furnishing,
                _ when t.IsAssignableFrom(typeof(GardeningNoti)) => NotiType.Gardening,
                _ => NotiType.Resin
            };
        }
    }
}
