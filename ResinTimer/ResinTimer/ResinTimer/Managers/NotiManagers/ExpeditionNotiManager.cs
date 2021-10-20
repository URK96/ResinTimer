using ResinTimer.Models.Notis;

using System;

using Xamarin.Forms;

namespace ResinTimer.Managers.NotiManagers
{
    public class ExpeditionNotiManager : NotiManager
    {
        const int ID_PREINDEX = 1000;

        public ExpeditionNotiManager() : base()
        {
            try
            {
                notiType = NotiType.Expedition;

                Notis.AddRange(GetNotiList<ExpeditionNoti>());
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize expedition noti manager");
            }
        }

        public override void UpdateNotisTime()
        {
            SaveNotis();
            UpdateScheduledNoti<ExpeditionNoti>();
        }

        public override void RenewalIds()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                Notis[i].NotiId = ID_PREINDEX + i;
            }
        }

        public override void EditList(Noti item, EditType type)
        {
            switch (type)
            {
                case EditType.Add:
                    item.NotiId = ID_PREINDEX + Notis.Count;

                    Notis.Add(item);
                    Notis.Sort(SortNotis);
                    break;
                case EditType.Remove:
                    Notis.Remove(Notis.Find(x => x.NotiId.Equals(item.NotiId)));
                    break;
                case EditType.Edit:
                    var index = Notis.FindIndex(x => x.NotiId.Equals(item.NotiId));

                    Notis[index] = item;
                    Notis[index].UpdateTime();
                    break;
                case EditType.EditOnlyTime:
                    var index2 = Notis.FindIndex(x => x.NotiId.Equals(item.NotiId));

                    Notis[index2] = item;
                    break;
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<ExpeditionNoti>();
        }
    }
}
