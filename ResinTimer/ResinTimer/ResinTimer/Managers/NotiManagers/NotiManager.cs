using Newtonsoft.Json;

using ResinTimer.Models.Notis;
using ResinTimer.Services;

using System;
using System.Collections.Generic;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace ResinTimer.Managers.NotiManagers
{
    public class NotiManager
    {
        public enum EditType { Add, Remove, Edit, EditOnlyTime }
        public enum NotificationType { Resin, Expedition, GatheringItem, Gadget, Furnishing, Checklist, RealmCurrency, RealmFriendship, Gardening }

        public NotificationType NotiType = NotificationType.Resin;

        public List<Noti> Notis { get; set; }

        public NotiManager()
        {
            Notis = new List<Noti>();
        }

        internal int SortNotis(Noti x, Noti y)
        {
            return x.NotiId.CompareTo(y.NotiId);
        }

        public void ReloadNotiList<T>() where T : Noti
        {
            Notis.Clear();
            Notis.AddRange(GetNotiList<T>());
        }

        public List<T> GetNotiList<T>() where T : Noti
        {
            List<T> result = new();

            try
            {
                string key = typeof(T) switch
                {
                    Type t when t == typeof(ResinNoti) => SettingConstants.NOTI_LIST,
                    Type t when t == typeof(RealmCurrencyNoti) => SettingConstants.REALM_CURRENCY_NOTI_LIST,
                    Type t when t == typeof(RealmFriendshipNoti) => SettingConstants.REALM_FRIENDSHIP_NOTI_LIST,
                    Type t when t == typeof(ExpeditionNoti) => SettingConstants.EXPEDITION_NOTI_LIST,
                    Type t when t == typeof(GatheringItemNoti) => SettingConstants.GATHERINGITEM_NOTI_LIST,
                    Type t when t == typeof(GadgetNoti) => SettingConstants.GADGET_NOTI_LIST,
                    Type t when t == typeof(FurnishingNoti) => SettingConstants.FURNISHING_NOTI_LIST,
                    Type t when t == typeof(GardeningNoti) => SettingConstants.GARDENING_NOTI_LIST,
                    Type t when t == typeof(ChecklistNoti) => SettingConstants.CHECKLIST_LIST,
                    _ => string.Empty
                };

                if (string.IsNullOrEmpty(key))
                {
                    return result;
                }

                string value = Preferences.Get(key, string.Empty);

                List<T> deserialized = JsonConvert.DeserializeObject<List<T>>(value);

                if (deserialized != null)
                {
                    result.AddRange(deserialized);
                }
            }
            catch { }

            return result;
        }

        public INotiScheduleService GetScheduledService() => DependencyService.Get<INotiScheduleService>();

        public void UpdateScheduledNoti<T>() where T : Noti
        {
            if (!Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                return;
            }

            INotiScheduleService scheduledService = GetScheduledService();

            scheduledService.Cancel<T>();
            RenewalIds();
            SaveNotis();
            scheduledService.Schedule<T>();
        }

        public void SaveNotis()
        {
            string key = NotiType switch
            {
                NotificationType.Resin => SettingConstants.NOTI_LIST,
                NotificationType.RealmCurrency => SettingConstants.REALM_CURRENCY_NOTI_LIST,
                NotificationType.RealmFriendship => SettingConstants.REALM_FRIENDSHIP_NOTI_LIST,
                NotificationType.Expedition => SettingConstants.EXPEDITION_NOTI_LIST,
                NotificationType.GatheringItem => SettingConstants.GATHERINGITEM_NOTI_LIST,
                NotificationType.Gadget => SettingConstants.GADGET_NOTI_LIST,
                NotificationType.Furnishing => SettingConstants.FURNISHING_NOTI_LIST,
                NotificationType.Gardening => SettingConstants.GARDENING_NOTI_LIST,
                _ => string.Empty
            };

            Preferences.Set(key, JsonConvert.SerializeObject(Notis));
        }

        public virtual void RenewalIds() { }
        public virtual void EditList(Noti item, EditType editType) { }
        public virtual void UpdateNotisTime() { }

        public virtual void RemoveAllSyncNotiItems<T>() where T : Noti
        {
            Notis.RemoveAll(x => x.IsSyncItem);

            SaveNotis();
            UpdateScheduledNoti<T>();
        }
    }
}
