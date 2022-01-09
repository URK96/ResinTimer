using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;

using Xamarin.Forms;

namespace ResinTimer.Managers.NotiManagers
{
    public class RealmCurrencyNotiManager : NotiManager
    {
        internal const int ID_PREINDEX = 500;

        public RealmCurrencyNotiManager() : base()
        {
            try
            {
                NotiType = NotificationType.RealmCurrency;

                Notis.AddRange(GetNotiList<RealmCurrencyNoti>());

                if (Notis.Count < 1)
                {
                    Notis.Add(new RealmCurrencyNoti(100));
                    SaveNotis();
                }
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize noti manager");
            }
        }

        public override void RenewalIds()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                Notis[i].NotiId = ID_PREINDEX + (Notis[i] as RealmCurrencyNoti).Percentage;
            }
        }

        public override void UpdateNotisTime()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                (Notis[i] as RealmCurrencyNoti).UpdateTime();
            }

            SaveNotis();
            UpdateScheduledNoti<RealmCurrencyNoti>();
        }

        public override void EditList(Noti item, EditType type)
        {
            var noti = item as RealmCurrencyNoti;

            switch (type)
            {
                case EditType.Add:
                    if (Notis.FindAll(x => (x as RealmCurrencyNoti).Percentage.Equals(noti.Percentage)).Count == 0)
                    {
                        Notis.Add(item);
                        Notis.Sort(SortNotis);
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_AlreadyExistToast_Message);
                    }
                    break;
                case EditType.Remove:
                    Notis.Remove(Notis.Find(x => (x as RealmCurrencyNoti).Percentage.Equals(noti.Percentage)));
                    break;
                case EditType.Edit:
                    (Notis[Notis.FindIndex(x => (x as RealmCurrencyNoti).Percentage.Equals(noti.Percentage))] as RealmCurrencyNoti).Percentage = noti.Percentage;
                    break;
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<RealmCurrencyNoti>();
        }
    }
}
