﻿using Newtonsoft.Json;

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
        public enum EditType { Add, Remove, Edit, EditOnlyTime }
        public enum NotiType { Resin, Expedition, GatheringItem, Gadget }

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
                else if (typeof(T) == typeof(GadgetNoti))
                {
                    value = Preferences.Get(SettingConstants.GADGET_NOTI_LIST, string.Empty);
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
                NotiType.Resin => SettingConstants.NOTI_LIST,
                NotiType.Expedition => SettingConstants.EXPEDITION_NOTI_LIST,
                NotiType.GatheringItem => SettingConstants.GATHERINGITEM_NOTI_LIST,
                NotiType.Gadget => SettingConstants.GADGET_NOTI_LIST,
                _ => string.Empty
            };

            Preferences.Set(key, JsonConvert.SerializeObject(Notis));
        }

        public virtual void RenewalIds() { }
        public virtual void EditList(Noti item, EditType editType) { }
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

        public override void EditList(Noti item, EditType type)
        {
            var noti = item as ResinNoti;

            switch (type)
            {
                case EditType.Add:
                    if (Notis.FindAll(x => (x as ResinNoti).Resin.Equals(noti.Resin)).Count == 0)
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
                    Notis.Remove(Notis.Find(x => (x as ResinNoti).Resin.Equals(noti.Resin)));
                    break;
                case EditType.Edit:
                    (Notis[Notis.FindIndex(x => (x as ResinNoti).Resin.Equals(noti.Resin))] as ResinNoti).Resin = noti.Resin;
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

    public class GatheringItemNotiManager : NotiManager
    {
        const int ID_PREINDEX = 1500;

        public GatheringItemNotiManager() : base()
        {
            try
            {
                notiType = NotiType.GatheringItem;

                Notis.AddRange(GetNotiList<GatheringItemNoti>());

                SaveNotis();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize GI noti manager");
            }
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
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<GatheringItemNoti>();
        }
    }

    public class GadgetNotiManager : NotiManager
    {
        const int ID_PREINDEX = 2500;

        public GadgetNotiManager() : base()
        {
            try
            {
                notiType = NotiType.Gadget;

                Notis.AddRange(GetNotiList<GadgetNoti>());

                SaveNotis();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Fail to initialize Gadget noti manager");
            }
        }

        public void UpdateNotisTime()
        {
            SaveNotis();
            UpdateScheduledNoti<GadgetNoti>();
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
                default:
                    break;
            }

            SaveNotis();
            UpdateScheduledNoti<GadgetNoti>();
        }
    }
}
