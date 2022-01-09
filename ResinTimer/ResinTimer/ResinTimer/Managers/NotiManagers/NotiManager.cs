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

        public List<T> GetNotiList<T>() where T : Noti
        {
            string value = string.Empty;
            List<T> result = new();

            try
            {
                if (typeof(T) == typeof(ResinNoti))
                {
                    value = Preferences.Get(SettingConstants.NOTI_LIST, string.Empty);
                }
                else if (typeof(T) == typeof(RealmCurrencyNoti))
                {
                    value = Preferences.Get(SettingConstants.REALM_CURRENCY_NOTI_LIST, string.Empty);
                }
                else if (typeof(T) == typeof(RealmFriendshipNoti))
                {
                    value = Preferences.Get(SettingConstants.REALM_FRIENDSHIP_NOTI_LIST, string.Empty);
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
                else if (typeof(T) == typeof(FurnishingNoti))
                {
                    value = Preferences.Get(SettingConstants.FURNISHING_NOTI_LIST, string.Empty);
                }
                else if (typeof(T) == typeof(GardeningNoti))
                {
                    value = Preferences.Get(SettingConstants.GARDENING_NOTI_LIST, string.Empty);
                }
                else if (typeof(T) == typeof(ChecklistNoti))
                {
                    value = Preferences.Get(SettingConstants.CHECKLIST_LIST, string.Empty);
                }

                List<T> deserialized = JsonConvert.DeserializeObject<List<T>>(value);

                if (deserialized != null)
                {
                    result.AddRange(deserialized);
                }
            }
            catch (Exception) { }

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
    }
}
