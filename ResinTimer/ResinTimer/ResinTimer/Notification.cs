﻿using System;

namespace ResinTimer
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

        public NotiManager.NotiType NotiType { get; set; }

        public void SetType<T>() where T : Noti
        {
            if (typeof(T) == typeof(ResinNoti))
            {
                NotiType = NotiManager.NotiType.Resin;
            }
            else if (typeof(T) == typeof(ExpeditionNoti))
            {
                NotiType = NotiManager.NotiType.Expedition;
            }
            else if (typeof(T) == typeof(GatheringItemNoti))
            {
                NotiType = NotiManager.NotiType.GatheringItem;
            }
            else if (typeof(T) == typeof(GadgetNoti))
            {
                NotiType = NotiManager.NotiType.Gadget;
            }
        }
    }
}
