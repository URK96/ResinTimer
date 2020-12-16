using Newtonsoft.Json;

using ResinTimer.Resources;

using System;
using System.Collections.Generic;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace ResinTimer
{
    public class NotiManager
    {
        public enum EditType { Add, Remove, Edit }

        public List<Noti> Notis { get; set; }

        public NotiManager()
        {
            try
            {
                Notis = new List<Noti>();

                var list = Preferences.Get(SettingConstants.NOTI_LIST, string.Empty);

                if (string.IsNullOrWhiteSpace(list))
                {
                    Notis.Add(new Noti(ResinEnvironment.MAX_RESIN));
                    SaveNotis();
                    UpdateScheduledNoti();
                }
                else
                {
                    Notis.AddRange(JsonConvert.DeserializeObject<List<Noti>>(list));
                }
            }
            catch (Exception)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    DependencyService.Get<IToast>().Show("Fail to initialize noti manager");
                }
            }
        }

        public void RenewalIds()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                Notis[i].NotiId = Notis[i].Resin;
            }
        }

        public void UpdateNotisTime()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                Notis[i].UpdateTime();
            }

            SaveNotis();
            UpdateScheduledNoti();
        }

        public void SaveNotis()
        {
            Preferences.Set(SettingConstants.NOTI_LIST, JsonConvert.SerializeObject(Notis));
        }

        public void EditList(Noti item, EditType type)
        {
            switch (type)
            {
                case EditType.Add:
                    if (Notis.FindAll(x => x.Resin.Equals(item.Resin)).Count == 0)
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
                    Notis.Remove(Notis.Find(x => x.Resin.Equals(item.Resin)));
                    break;
                case EditType.Edit:
                    Notis[Notis.FindIndex(x => x.Resin.Equals(item.Resin))].Resin = item.Resin;
                    break;
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti();
        }

        public void UpdateScheduledNoti()
        {
            var service = DependencyService.Get<IScheduledNoti>();

            service.CancelAll();
            RenewalIds();
            service.ScheduleAllNoti();
        }

        private int SortNotis(Noti x, Noti y)
        {
            return x.NotiId.CompareTo(y.NotiId);
        }
    }
}
