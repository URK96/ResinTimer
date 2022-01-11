using ResinTimer.Models.Notis;

using System;

using Xamarin.Forms;

namespace ResinTimer.Managers.NotiManagers
{
    public class GardeningNotiManager : NotiManager
    {
        const int IdPreIndex = 3900;

        public GardeningNotiManager() : base()
        {
            try
            {
                NotiType = NotificationType.Gardening;

                Notis.AddRange(GetNotiList<GardeningNoti>());

                SaveNotis();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize Gardening noti manager");
            }
        }

        public override void UpdateNotisTime()
        {
            SaveNotis();
            UpdateScheduledNoti<GardeningNoti>();
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
                    int index = Notis.FindIndex(x => x.NotiId == item.NotiId);

                    Notis[index] = item;
                    Notis[index].UpdateTime();
                    break;
                case EditType.EditOnlyTime:
                    Notis[Notis.FindIndex(x => x.NotiId == item.NotiId)] = item;
                    break;
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<GardeningNoti>();
        }
    }
}
