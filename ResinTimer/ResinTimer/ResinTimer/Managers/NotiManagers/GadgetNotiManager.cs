using ResinTimer.Models.Notis;

using System;

using Xamarin.Forms;

namespace ResinTimer.Managers.NotiManagers
{
    public class GadgetNotiManager : NotiManager
    {
        const int IdPreIndex = 2500;

        public GadgetNotiManager() : base()
        {
            try
            {
                NotiType = NotificationType.Gadget;

                Notis.AddRange(GetNotiList<GadgetNoti>());

                SaveNotis();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize Gadget noti manager");
            }
        }

        public override void UpdateNotisTime()
        {
            SaveNotis();
            UpdateScheduledNoti<GadgetNoti>();
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
            UpdateScheduledNoti<GadgetNoti>();
        }
    }
}
