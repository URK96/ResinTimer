using GenshinDB_Core.Types;

using GenshinInfo.Managers;
using GenshinInfo.Models;

using ResinTimer.Models.Materials;
using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;

using static ResinTimer.AppEnvironment;

using RealmEnv = ResinTimer.RealmEnvironment;

namespace ResinTimer
{
    public static class ResinEnvironment
    {
        public enum ApplyType { Time = 0, Resin }

        public static ApplyType ManualApplyType = ApplyType.Time;

        public const int MaxResin = 160;
        public const int OneRestoreInterval = 480;

        public static DateTime EndTime;
        public static string LastInputTime;
        public static TimeSpan TotalCountTime;
        public static TimeSpan OneCountTime;

        public static int Resin = 0;

        public static bool IsSyncEnabled => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_ENABLED, false) &&
            Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_RESIN_ENABLED, false);

        public static void LoadValues()
        {
            InitAppLang();

            if (Preferences.ContainsKey(SettingConstants.END_TIME))
            {
                string endTimeP = Preferences.Get(SettingConstants.END_TIME, "");

                if (!DateTime.TryParse(endTimeP, DTCulture, System.Globalization.DateTimeStyles.None, out EndTime))
                {
                    EndTime = DateTime.Now;
                }
            }
            else
            {
                DateTime now = DateTime.Now;

                Preferences.Set(SettingConstants.END_TIME, now.ToString(DTCulture));

                EndTime = now;
            }

            LastInputTime = Preferences.Get(SettingConstants.LAST_INPUT_TIME, DateTime.Now.ToString(DTCulture));
            ManualApplyType = (ApplyType)Preferences.Get(SettingConstants.RESIN_INPUT_TYPE, (int)ApplyType.Time);

            if (Preferences.ContainsKey(SettingConstants.RESIN_COUNT))
            {
                Resin = Preferences.Get(SettingConstants.RESIN_COUNT, 0);
            }
            else
            {
                Resin = 0;

                Preferences.Set(SettingConstants.RESIN_COUNT, Resin);
            }
        }

        public static void CalcResinTime()
        {
            TotalCountTime = EndTime - DateTime.Now;
            OneCountTime = TimeSpan.FromSeconds(TotalCountTime.TotalSeconds % OneRestoreInterval);
        }

        public static void CalcResin()
        {
            DateTime now = DateTime.Now;

            Resin = (EndTime <= now) ? MaxResin :
                MaxResin - (Convert.ToInt32((EndTime - now).TotalSeconds) / OneRestoreInterval) - 1;
        }

        public static void SaveValue()
        {
            try
            {
                Preferences.Set(SettingConstants.RESIN_COUNT, Resin);
                Preferences.Set(SettingConstants.END_TIME, EndTime.ToString(DTCulture));
                Preferences.Set(SettingConstants.LAST_INPUT_TIME, LastInputTime);
                Preferences.Set(SettingConstants.RESIN_INPUT_TYPE, (int)ManualApplyType);
            }
            catch { }
        }

        public static int CalcResinOverflow()
        {
            DateTime now = DateTime.Now;

            return now switch
            {
                _ when now > EndTime => (int)(now - EndTime).TotalSeconds / OneRestoreInterval,
                _ => -1
            };
        }

        public static async Task<bool> SyncServerData()
        {
            try
            {
                GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);
                RTNoteData data = await manager.GetRealTimeNotes();

                if (data is not null)
                {
                    TimeSpan addInterval = data.ResinRecoveryTime;

                    if (data.CurrentResin > MaxResin)
                    {
                        addInterval = TimeSpan.FromMinutes(-(data.CurrentResin - MaxResin) * 8);
                    }

                    DateTime now = DateTime.Now;

                    EndTime = now.Add(addInterval);
                    LastInputTime = now.ToString(DTCulture);

                    CalcResin();
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public static class RealmEnvironment
    {
        public enum RealmRankEnum
        {
            BareBones,
            HumbleAbode,
            Cozy,
            QueenSize,
            Elegant,
            Exquisite,
            Extraordinary,
            Stately,
            Luxury,
            KingFit
        }

        public enum RealmType { Currency = 0, Friendship }

        public static RealmRankEnum RealmRank = RealmRankEnum.BareBones;
        public static int TrustRank = 1;

        public static void LoadValues()
        {
            RealmRank = (RealmRankEnum)Preferences.Get(SettingConstants.REALM_ADEPTAL_LEVEL, 0);
            TrustRank = Preferences.Get(SettingConstants.REALM_TRUST_RANK, 1);
        }

        public static void SaveValues()
        {
            Preferences.Set(SettingConstants.REALM_ADEPTAL_LEVEL, (int)RealmRank);
            Preferences.Set(SettingConstants.REALM_TRUST_RANK, TrustRank);
        }
    }

    public static class RealmCurrencyEnvironment
    {
        public const int HourToSec = 3600;
        public const int MinToSec = 60;  // Only use test

        public static int MaxRC => IsSyncEnabled ? s_serverMaxRC : s_rcCapacity[RealmEnv.TrustRank - 1];
        public static int RCRate => s_aeRate[(int)RealmEnv.RealmRank];
        public static int Percentage => Convert.ToInt32((double)Currency / MaxRC * 100);

        private static readonly int[] s_aeRate = { 4, 8, 12, 16, 20, 22, 24, 26, 28, 30 };
        private static readonly int[] s_rcCapacity = { 300, 600, 900, 1200, 1400, 1600, 1800, 2000, 2200, 2400 };

        public static DateTime EndTime;
        public static int AddCount = 0;
        public static int Currency = 0;
        public static string LastInputTime;
        public static TimeSpan TotalCountTime;
        public static TimeSpan OneCountTime;

        public static bool IsSyncEnabled => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_ENABLED, false) &&
            Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_REALMCURRENCY_ENABLED, false);

        private static int s_serverMaxRC = s_rcCapacity[RealmEnv.TrustRank - 1];

        public static void LoadValues()
        {
            InitAppLang();

            if (Preferences.ContainsKey(SettingConstants.RC_END_TIME))
            {
                string endTimeP = Preferences.Get(SettingConstants.RC_END_TIME, "");

                if (!DateTime.TryParse(endTimeP, DTCulture, System.Globalization.DateTimeStyles.None, out EndTime))
                {
                    EndTime = DateTime.Now;
                }
            }
            else
            {
                DateTime now = DateTime.Now;

                Preferences.Set(SettingConstants.RC_END_TIME, now.ToString(DTCulture));

                EndTime = now;
            }

            LastInputTime = Preferences.Get(SettingConstants.RC_LAST_INPUT_TIME, DateTime.Now.ToString(DTCulture));
            Currency = Preferences.Get(SettingConstants.RC_COUNT, 0);
            AddCount = Preferences.Get(SettingConstants.RC_ADD_COUNT, 0);

            RealmEnv.LoadValues();
        }

        public static void CalcRC()
        {
            DateTime now = DateTime.Now;

            if (EndTime <= now)
            {
                Currency = MaxRC;
            }
            else
            {
                DateTime lastDT = DateTime.Parse(LastInputTime, DTCulture);

#if TEST
                int count = (int)(now - lastDT.AddMinutes(addCount)).TotalSeconds / M_TO_S;
#else
                int count = (int)(now - lastDT.AddHours(AddCount)).TotalSeconds / HourToSec;
#endif

                if (count > 0)
                {
                    Currency += RCRate * count;
                    AddCount += count;
                }
            }

            CalcReaminTime();
        }

        private static void CalcReaminTime()
        {
            DateTime now = DateTime.Now;

            TotalCountTime = (EndTime > now) ? (EndTime - now) : TimeSpan.FromSeconds(0);
        }

        public static void CalcEndTime()
        {
            int remainCurrency = MaxRC - Currency;
            int remainCount = remainCurrency / RCRate;
            DateTime lastDT = DateTime.Parse(LastInputTime, DTCulture);

            remainCount += (remainCurrency % RCRate) == 0 ? 0 : 1;

#if TEST
            EndTime = lastDT.AddMinutes(AddCount).AddMinutes(remainCount);
#else
            EndTime = lastDT.AddHours(AddCount).AddHours(remainCount);
#endif
        }

        public static async Task<bool> SyncServerData()
        {
            try
            {
                GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);
                RTNoteData data = await manager.GetRealTimeNotes();

                if (data is not null)
                {
                    TimeSpan addInterval = data.HomeCoinRecoveryTime;

                    s_serverMaxRC = data.MaxHomeCoin;

                    if (data.CurrentHomeCoin >= s_serverMaxRC)
                    {
                        addInterval = TimeSpan.FromSeconds(0);
                    }

                    DateTime now = DateTime.Now;

                    Currency = data.CurrentHomeCoin;
                    EndTime = now.Add(addInterval);
                    LastInputTime = now.ToString(DTCulture);

                    CalcReaminTime();
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static void SaveValue()
        {
            try
            {
                Preferences.Set(SettingConstants.RC_COUNT, Currency);
                Preferences.Set(SettingConstants.RC_ADD_COUNT, AddCount);
                Preferences.Set(SettingConstants.RC_END_TIME, EndTime.ToString(DTCulture));
                Preferences.Set(SettingConstants.RC_LAST_INPUT_TIME, LastInputTime);

                RealmEnv.SaveValues();
            }
            catch { }
        }
    }

    public static class RealmFriendshipEnvironment
    {
        public const int HourToSec = 3600;
        public const int MinToSec = 60;  // Only use test

        public static int MaxRF => s_rfCapacity[RealmEnv.TrustRank - 1];
        public static int RFRate => s_aeRate[(int)RealmEnv.RealmRank];
        public static int Percentage => Convert.ToInt32((double)Bounty / MaxRF * 100);

        private static readonly int[] s_aeRate = { 2, 2, 3, 3, 4, 4, 4, 5, 5, 5 };
        private static readonly int[] s_rfCapacity = { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500 };

        public static DateTime EndTime;
        public static int AddCount = 0;
        public static int Bounty = 0;
        public static string LastInputTime;
        public static TimeSpan TotalCountTime;
        public static TimeSpan OneCountTime;

        public static void LoadValues()
        {
            InitAppLang();

            if (Preferences.ContainsKey(SettingConstants.RF_END_TIME))
            {
                string endTimeP = Preferences.Get(SettingConstants.RF_END_TIME, "");

                if (!DateTime.TryParse(endTimeP, DTCulture, System.Globalization.DateTimeStyles.None, out EndTime))
                {
                    EndTime = DateTime.Now;
                }
            }
            else
            {
                DateTime now = DateTime.Now;

                Preferences.Set(SettingConstants.RF_END_TIME, now.ToString(DTCulture));

                EndTime = now;
            }

            LastInputTime = Preferences.Get(SettingConstants.RF_LAST_INPUT_TIME, DateTime.Now.ToString(DTCulture));
            Bounty = Preferences.Get(SettingConstants.RF_COUNT, 0);
            AddCount = Preferences.Get(SettingConstants.RF_ADD_COUNT, 0);

            RealmEnv.LoadValues();
        }

        public static void CalcRF()
        {
            DateTime now = DateTime.Now;

            if (EndTime <= now)
            {
                Bounty = MaxRF;
            }
            else
            {
                DateTime lastDT = DateTime.Parse(LastInputTime, DTCulture);

#if TEST
                int count = (int)(now - lastDT.AddMinutes(AddCount)).TotalSeconds / M_TO_S;
#else
                int count = (int)(now - lastDT.AddHours(AddCount)).TotalSeconds / HourToSec;
#endif

                if (count > 0)
                {
                    Bounty += RFRate * count;
                    AddCount += count;
                }
            }

            CalcRemainTime();
        }

        private static void CalcRemainTime()
        {
            var now = DateTime.Now;

            TotalCountTime = (EndTime > now) ? (EndTime - now) : TimeSpan.FromSeconds(0);
        }

        public static void CalcEndTime()
        {
            int remainCurrency = MaxRF - Bounty;
            int remainCount = remainCurrency / RFRate;
            DateTime lastDT = DateTime.Parse(LastInputTime, DTCulture);

            remainCount += (remainCurrency % RFRate) == 0 ? 0 : 1;

#if TEST
            EndTime = lastDT.AddMinutes(AddCount).AddMinutes(remainCount);
#else
            EndTime = lastDT.AddHours(AddCount).AddHours(remainCount);
#endif
        }

        public static void SaveValue()
        {
            try
            {
                Preferences.Set(SettingConstants.RF_COUNT, Bounty);
                Preferences.Set(SettingConstants.RF_ADD_COUNT, AddCount);
                Preferences.Set(SettingConstants.RF_END_TIME, EndTime.ToString(DTCulture));
                Preferences.Set(SettingConstants.RF_LAST_INPUT_TIME, LastInputTime);

                RealmEnv.SaveValues();
            }
            catch { }
        }
    }

    public static class ExpeditionEnvironment
    {
        public enum ExpeditionType { Chunk = 0, Ingredient, Mora }
    }

    public static class GatheringItemEnvironment
    {
        public enum GItemType 
        { 
            MagicCrystalChunk = 0, 
            Artifact, 
            Specialty, 
            Artifact12H, 
            CrystalChunk, 
            WhiteIronChunk, 
            IronChunk,
            ElectroCrystal,
            CrystalCore,
            AmethystLump
        }

        public static double[] ResetTimeList = { 72, 24, 48, 12, 72, 48, 24, 48, 12, 72 };
    }

    public static class GadgetEnvironment
    {
        public enum GadgetType { ParametricTransformer = 0, PortableWaypoint }

        public static double[] ResetTimeList = { 166, 168 };
    }

    public static class FurnishingEnvironment
    {
        public const int SpeedUpHour = 4;

        public enum FurnishType { Rarity2, Rarity3, Rarity4 }

        public static double[] ResetTimeList = { 12, 14, 16 }; // Rarity : 2 3 4
    }

    public static class GardeningEnvironment
    {
        public const int FullGrowInterval = 70;

        public enum FieldType { JadeField, LuxuriantGlebe, OrderlyMeadow }
    }

    public static class ChecklistEnvironment
    {
        public enum ResetType { Once = 0, Custom, Daily, Weekly, Monthly }
    }

    public class TalentEnvironment
    {
        public const int RenewalHour = 4;

        public static TalentEnvironment Instance => s_instance.Value;

        private static readonly Lazy<TalentEnvironment> s_instance = 
            new(() => new TalentEnvironment());

        public List<IMaterialItem> Items { get; }

        private TalentEnvironment()
        {
            Items = new();
        }

        public void UpdateNowTalentBooks()
        {
            GDB ??= new(AppResources.Culture);

            DayOfWeek dayOfWeekValue = Utils.GetServerDayOfWeek();

            var items = from item in GDB.talentItems
                        where item.AvailableDayOfWeeks.Contains(dayOfWeekValue)
                        select item;

            Items.Clear();

            foreach (TalentItem item in items)
            {
                Items.Add(new TalentListItem(item));
            }
        }
    }

    public class WeaponAscensionEnvironment
    {
        public const int RenewalHour = 4;

        public static WeaponAscensionEnvironment Instance => s_instance.Value;

        private static readonly Lazy<WeaponAscensionEnvironment> s_instance =
            new(() => new WeaponAscensionEnvironment());

        public List<IMaterialItem> Items { get; }

        private WeaponAscensionEnvironment()
        {
            Items = new();
        }

        public void UpdateNowWAItems()
        {
            GDB ??= new(AppResources.Culture);

            DayOfWeek dayOfWeekValue = Utils.GetServerDayOfWeek();

            var items = from item in GDB.weaponAscensionItems
                        where item.AvailableDayOfWeeks.Contains(dayOfWeekValue)
                        select item;

            Items.Clear();

            foreach (WeaponAscensionItem item in items)
            {
                Items.Add(new WAListItem(item));
            }
        }
    }
}
