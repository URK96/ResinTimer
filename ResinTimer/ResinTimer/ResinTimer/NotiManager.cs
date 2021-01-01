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
        public enum NotiType { Resin, Expedition }

        public NotiType notiType = NotiType.Resin;

        public List<Noti> Notis { get; set; }

        private IScheduledNoti ScheduledService => DependencyService.Get<IScheduledNoti>();

        public NotiManager()
        {
            Notis = new List<Noti>();
        }

        internal int SortNotis(Noti x, Noti y)
        {
            return x.NotiId.CompareTo(y.NotiId);
        }

        public List<T> GetNotiList<T>() where T : Noti
        {
            string value = string.Empty;
            List<T> result;

            try
            {
                if (typeof(T) == typeof(ResinNoti))
                {
                    value = Preferences.Get(SettingConstants.NOTI_LIST, string.Empty);
                }
                else if (typeof(T) == typeof(ExpeditionNoti))
                {
                    value = Preferences.Get(SettingConstants.EXPEDITION_NOTI_LIST, string.Empty);
                }

                result = JsonConvert.DeserializeObject<List<T>>(value);

                if (result == null)
                {
                    result = new List<T>();
                }
            }
            catch (Exception)
            {
                result = new List<T>();

                return result;
            }

            return result;
        }

        public void UpdateScheduledNoti<T>() where T : Noti
        {
            ScheduledService.Cancel<T>();
            RenewalIds();
            SaveNotis();
            ScheduledService.Schedule<T>();
        }

        public void SaveNotis()
        {
            var key = notiType switch
            {
                NotiType.Expedition => SettingConstants.EXPEDITION_NOTI_LIST,
                _ => SettingConstants.NOTI_LIST
            };

            Preferences.Set(key, JsonConvert.SerializeObject(Notis));
        }

        public virtual void RenewalIds() { }
    }

    public class ResinNotiManager : NotiManager
    {
        public ResinNotiManager() : base()
        {
            try
            {
                notiType = NotiType.Resin;

                //var list = Preferences.Get(SettingConstants.NOTI_LIST, string.Empty);

                //if (string.IsNullOrWhiteSpace(list))
                //{
                //    Notis.Add(new ResinNoti(ResinEnvironment.MAX_RESIN));
                //    SaveNotis();
                //}
                //else
                //{
                //    Notis.AddRange(JsonConvert.DeserializeObject<List<ResinNoti>>(list));
                //}

                Notis.AddRange(GetNotiList<ResinNoti>());

                if (Notis.Count < 1)
                {
                    Notis.Add(new ResinNoti(ResinEnvironment.MAX_RESIN));
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
                Notis[i].NotiId = (Notis[i] as ResinNoti).Resin;
            }
        }

        public void UpdateNotisTime()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                (Notis[i] as ResinNoti).UpdateTime();
            }

            SaveNotis();
            UpdateScheduledNoti<ResinNoti>();
        }

        public void EditList(ResinNoti item, EditType type)
        {
            switch (type)
            {
                case EditType.Add:
                    if (Notis.FindAll(x => (x as ResinNoti).Resin.Equals(item.Resin)).Count == 0)
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
                    Notis.Remove(Notis.Find(x => (x as ResinNoti).Resin.Equals(item.Resin)));
                    break;
                case EditType.Edit:
                    (Notis[Notis.FindIndex(x => (x as ResinNoti).Resin.Equals(item.Resin))] as ResinNoti).Resin = item.Resin;
                    break;
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<ResinNoti>();
        }
    }

    public class ExpeditionNotiManager : NotiManager
    {
        const int ID_PREINDEX = 1000;

        public ExpeditionNotiManager() : base()
        {
            try
            {
                notiType = NotiType.Expedition;

                //var list = Preferences.Get(SettingConstants.EXPEDITION_NOTI_LIST, string.Empty);

                //if (!string.IsNullOrWhiteSpace(list))
                //{
                //    Notis.AddRange(JsonConvert.DeserializeObject<List<ExpeditionNoti>>(list));
                //}

                Notis.AddRange(GetNotiList<ExpeditionNoti>());
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize expedition noti manager");
            }
        }

        public void UpdateNotisTime()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                //Notis[i].UpdateTime();
            }

            UpdateScheduledNoti<ExpeditionNoti>();
        }

        public override void RenewalIds()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                Notis[i].NotiId = ID_PREINDEX + i;
            }
        }

        public void EditList(Noti item, EditType type)
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
                default:
                    break;
            }
            
            UpdateScheduledNoti<ExpeditionNoti>();
        }
    }
}
