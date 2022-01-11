﻿using ResinTimer.Managers.NotiManagers;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer.Models.HomeItems
{
    public abstract class ListTimerHomeItem : IHomeItem
    {
        public string StatusMessage
        {
            get
            {
                int finishCount = (from noti in _manager.Notis
                                   where noti.NotiTime <= DateTime.Now
                                   select noti).Count();

                return $"{finishCount} / {_manager.Notis.Count}";
            }
        }

        public virtual string OptionalMessage => string.Empty;

        public abstract string ImageString { get; }

        private readonly NotiManager _manager;

        public ListTimerHomeItem(NotiManager manager)
        {
            _manager = manager;
        }
    }
}
