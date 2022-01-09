using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResinTimer.Models.HomeItems
{
    public class ExpeditionHomeItem : IHomeItem
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

        public string OptionalMessage => string.Empty;

        public string ImageString => "compass.png";

        private readonly NotiManager _manager;

        public ExpeditionHomeItem()
        {
            _manager = new ExpeditionNotiManager();
        }
    }
}
