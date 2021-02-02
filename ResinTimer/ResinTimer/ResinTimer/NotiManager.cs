using Newtonsoft.Json;

using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Essentials;
using Xamarin.Forms;

using GIEnv = ResinTimer.GatheringItemEnvironment;

namespace ResinTimer
{
    public class NotiManager
    {
        public enum EditType { Add, Remove, Edit }
        public enum NotiType { Resin, Expedition, GatheringItem }

        public NotiType notiType = NotiType.Resin;

        public List<Noti> Notis { get; set; }

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
            var result = new List<T>();

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
                else if (typeof(T) == typeof(GatheringItemNoti))
                {
                    value = Preferences.Get(SettingConstants.GATHERINGITEM_NOTI_LIST, string.Empty);
                }

                var deserialized = JsonConvert.DeserializeObject<List<T>>(value);

                if (deserialized != null)
                {
                    result.AddRange(deserialized);
                }
            }
            catch (Exception) { }

            return result;
        }

        public IScheduledNoti GetScheduledService() => DependencyService.Get<IScheduledNoti>();

        public void UpdateScheduledNoti<T>() where T : Noti
        {
            if (!Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                return;
            }

            var scheduledService = GetScheduledService();

            scheduledService.Cancel<T>();
            RenewalIds();
            SaveNotis();
            scheduledService.Schedule<T>();
        }

        public void SaveNotis()
        {
            var key = notiType switch
            {
                NotiType.Expedition => SettingConstants.EXPEDITION_NOTI_LIST,
                NotiType.GatheringItem => SettingConstants.GATHERINGITEM_NOTI_LIST,
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

    public class GatheringItemNotiManager : NotiManager
    {
        const int ID_PREINDEX = 1500;

        public GatheringItemNotiManager() : base()
        {
            try
            {
                notiType = NotiType.GatheringItem;

                var list = GetNotiList<GatheringItemNoti>();

                Notis.AddRange(list);

                if (list.Count < GIEnv.TypeCount)
                {
                    Notis.AddRange(CheckItemList(list));
                }

                SaveNotis();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize GI noti manager");
            }
        }

        private List<GatheringItemNoti> CheckItemList(List<GatheringItemNoti> existList)
        {
            var list = new List<GatheringItemNoti>();
            var checkList = Enumerable.Repeat(false, existList.Count).ToArray();

            try
            {
                list.Add(new GatheringItemNoti(GIEnv.GItemType.Chunk));
                list.Add(new GatheringItemNoti(GIEnv.GItemType.Artifact));
                list.Add(new GatheringItemNoti(GIEnv.GItemType.Specialty));
                list.Add(new GatheringItemNoti(GIEnv.GItemType.Artifact12H));

                foreach (var noti in existList)
                {
                    checkList[(int)noti.ItemType] = true;
                }

                for (int i = 0; i < checkList.Length; ++i)
                {
                    if (!checkList[i])
                    {
                        list.Add(new GatheringItemNoti((GIEnv.GItemType)i));
                    }
                }
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize GI list");
            }

            return list;
        }

        public void UpdateNotisTime()
        {
            SaveNotis();
            UpdateScheduledNoti<GatheringItemNoti>();
        }

        public override void RenewalIds()
        {
            for (int i = 0; i < Notis.Count; ++i)
            {
                Notis[i].NotiId = ID_PREINDEX + i;
            }
        }

        //public void EditList(Noti item, EditType type)
        //{
        //    switch (type)
        //    {
        //        case EditType.Add:
        //            item.NotiId = ID_PREINDEX + Notis.Count;

        //            Notis.Add(item);
        //            Notis.Sort(SortNotis);
        //            break;
        //        case EditType.Remove:
        //            Notis.Remove(Notis.Find(x => x.NotiId.Equals(item.NotiId)));
        //            break;
        //        case EditType.Edit:
        //            var index = Notis.FindIndex(x => x.NotiId.Equals(item.NotiId));

        //            Notis[index] = item;
        //            Notis[index].UpdateTime();
        //            break;
        //        default:
        //            break;
        //    }

        //    UpdateScheduledNoti<ExpeditionNoti>();
        //}
    }
}
