using ResinTimer.Models.Notis;

using System;

using Xamarin.Forms;

namespace ResinTimer.Managers.NotiManagers
{
    public class GatheringItemNotiManager : NotiManager
    {
        const int IdPreIndex = 1500;

        public GatheringItemNotiManager() : base()
        {
            try
            {
                NotiType = NotificationType.GatheringItem;

                Notis.AddRange(GetNotiList<GatheringItemNoti>());

                SaveNotis();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize GI noti manager");
            }
        }

        public override void UpdateNotisTime()
        {
            SaveNotis();
            UpdateScheduledNoti<GatheringItemNoti>();
        }

        public override void RenewalIds()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                Notis[i].NotiId = IdPreIndex + i;
            }
        }

        public override void EditList(Noti item, EditType type)
        {
            switch (type)
            {
                case EditType.Add:
                    item.NotiId = IdPreIndex + Notis.Count;

                    Notis.Add(item);
                    Notis.Sort(SortNotis);
                    break;
                case EditType.Remove:
                    Notis.Remove(Notis.Find(x => x.NotiId == item.NotiId));
                    break;
                case EditType.Edit:
                    var index = Notis.FindIndex(x => x.NotiId == item.NotiId);

                    Notis[index] = item;
                    Notis[index].UpdateTime();
                    break;
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<GatheringItemNoti>();
        }
    }
}
