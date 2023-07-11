using ResinTimer.Models.Notis;

using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer.Services
{
    public class NotiScheduleService : INotiScheduleService
    {
        public virtual void Cancel<T>() where T : Noti { }
        public virtual void Schedule<T>() where T : Noti { }
        public virtual void ScheduleNotiItem<T>(Noti noti) where T : Noti { }
        public virtual void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime) { }
        public virtual void TestNoti(string message = "") { }

        public void ScheduleAll()
        {
            Schedule<ResinNoti>();
            Schedule<RealmCurrencyNoti>();
            Schedule<RealmFriendshipNoti>();
            Schedule<ExpeditionNoti>();
            Schedule<GatheringItemNoti>();
            Schedule<GadgetNoti>();
            Schedule<FurnishingNoti>();
            Schedule<GardeningNoti>();
        }

        public virtual void CancelAll()
        {
            Cancel<ResinNoti>();
            Cancel<RealmCurrencyNoti>();
            Cancel<RealmFriendshipNoti>();
            Cancel<ExpeditionNoti>();
            Cancel<GatheringItemNoti>();
            Cancel<GadgetNoti>();
            Cancel<FurnishingNoti>();
            Cancel<GardeningNoti>();
        }
    }
}
