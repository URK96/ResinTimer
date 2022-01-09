using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;

using Xamarin.Forms;

namespace ResinTimer.Managers.NotiManagers
{
    public class RealmFriendshipNotiManager : NotiManager
    {
        internal const int ID_PREINDEX = 700;

        public RealmFriendshipNotiManager() : base()
        {
            try
            {
                NotiType = NotificationType.RealmFriendship;

                Notis.AddRange(GetNotiList<RealmFriendshipNoti>());

                if (Notis.Count < 1)
                {
                    Notis.Add(new RealmFriendshipNoti(100));
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
                Notis[i].NotiId = ID_PREINDEX + (Notis[i] as RealmFriendshipNoti).Percentage;
            }
        }

        public override void UpdateNotisTime()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                (Notis[i] as RealmFriendshipNoti).UpdateTime();
            }

            SaveNotis();
            UpdateScheduledNoti<RealmFriendshipNoti>();
        }

        public override void EditList(Noti item, EditType type)
        {
            var noti = item as RealmFriendshipNoti;

            switch (type)
            {
                case EditType.Add:
                    if (Notis.FindAll(x => (x as RealmFriendshipNoti).Percentage.Equals(noti.Percentage)).Count == 0)
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
                    Notis.Remove(Notis.Find(x => (x as RealmFriendshipNoti).Percentage.Equals(noti.Percentage)));
                    break;
                case EditType.Edit:
                    (Notis[Notis.FindIndex(x => (x as RealmFriendshipNoti).Percentage.Equals(noti.Percentage))] as RealmFriendshipNoti).Percentage = noti.Percentage;
                    break;
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<RealmFriendshipNoti>();
        }
    }
}
