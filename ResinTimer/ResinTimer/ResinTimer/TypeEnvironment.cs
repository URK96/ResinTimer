﻿using GenshinDB_Core;
using GenshinDB_Core.Types;

using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Essentials;
using Xamarin.Forms;

using static GenshinDB_Core.GenshinDB;
using static ResinTimer.AppEnvironment;

namespace ResinTimer
{
    public static class ResinEnvironment
    {
        public enum ApplyType { Time = 0, Resin }

        public static ApplyType applyType = ApplyType.Time;

        public const int MAX_RESIN = 160;

        public static DateTime endTime;
        public static string lastInputTime;
        public static ResinTime totalCountTime;
        public static ResinTime oneCountTime;

        public static int resin = 0;

        public static void LoadValues()
        {
            InitAppLang();

            if (Preferences.ContainsKey(SettingConstants.END_TIME))
            {
                string endTimeP = Preferences.Get(SettingConstants.END_TIME, "");

                if (!DateTime.TryParse(endTimeP, dtCulture, System.Globalization.DateTimeStyles.None, out endTime))
                {
                    endTime = DateTime.Now;
                }
            }
            else
            {
                var now = DateTime.Now;

                Preferences.Set(SettingConstants.END_TIME, now.ToString(dtCulture));
                endTime = now;
            }

            lastInputTime = Preferences.Get(SettingConstants.LAST_INPUT_TIME, DateTime.Now.ToString(dtCulture));
            applyType = (ApplyType)Preferences.Get(SettingConstants.RESIN_INPUT_TYPE, (int)ApplyType.Time);

            if (Preferences.ContainsKey(SettingConstants.RESIN_COUNT))
            {
                resin = Preferences.Get(SettingConstants.RESIN_COUNT, 0);
            }
            else
            {
                resin = 0;
                Preferences.Set(SettingConstants.RESIN_COUNT, resin);
            }
        }

        public static void CalcResin()
        {
            var now = DateTime.Now;

            if (endTime <= now)
            {
                resin = MAX_RESIN;
            }
            else
            {
                resin = MAX_RESIN - (Convert.ToInt32((endTime - now).TotalSeconds) / ResinTime.ONE_RESTORE_INTERVAL) - 1;
            }
        }

        public static void SaveValue()
        {
            try
            {
                Preferences.Set(SettingConstants.RESIN_COUNT, resin);
                Preferences.Set(SettingConstants.END_TIME, endTime.ToString(dtCulture));
                Preferences.Set(SettingConstants.LAST_INPUT_TIME, lastInputTime);
                Preferences.Set(SettingConstants.RESIN_INPUT_TYPE, (int)applyType);
            }
            catch { }
        }
    }

    public static class RealmCurrencyEnvironment
    {
        public const int H_TO_S = 3600;
        public const int M_TO_S = 60;  // Only use test

        public enum RealmRank
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

        public static int MaxRC => rcCapacity[trustRank - 1];
        public static int RCRate => aeRate[(int)realmRank];

        public static RealmRank realmRank = RealmRank.BareBones;

        public static int[] aeRate = { 4, 8, 12, 16, 20, 22, 24, 26, 28, 30 };
        public static int[] rcCapacity = { 300, 600, 900, 1200, 1400, 1600, 1800, 2000, 2200, 2400 };

        public static DateTime endTime;
        public static int addCount = 0;
        public static string lastInputTime;
        public static TimeSpan totalCountTime;
        public static TimeSpan oneCountTime;

        public static int currency = 0;
        public static int trustRank = 1;

        public static void LoadValues()
        {
            InitAppLang();

            if (Preferences.ContainsKey(SettingConstants.RC_END_TIME))
            {
                string endTimeP = Preferences.Get(SettingConstants.RC_END_TIME, "");

                if (!DateTime.TryParse(endTimeP, dtCulture, System.Globalization.DateTimeStyles.None, out endTime))
                {
                    endTime = DateTime.Now;
                }
            }
            else
            {
                var now = DateTime.Now;

                Preferences.Set(SettingConstants.RC_END_TIME, now.ToString(dtCulture));
                endTime = now;
            }

            lastInputTime = Preferences.Get(SettingConstants.RC_LAST_INPUT_TIME, DateTime.Now.ToString(dtCulture));
            realmRank = (RealmRank)Preferences.Get(SettingConstants.RC_ADEPTAL_LEVEL, 0);
            trustRank = Preferences.Get(SettingConstants.RC_TRUST_RANK, 1);
            currency = Preferences.Get(SettingConstants.RC_COUNT, 0);
            addCount = Preferences.Get(SettingConstants.RC_ADD_COUNT, 0);
        }

        public static void CalcRC()
        {
            var now = DateTime.Now;

            if (endTime <= now)
            {
                currency = MaxRC;
            }
            else
            {
                var lastDT = DateTime.Parse(lastInputTime, dtCulture);

#if TEST
                int count = (int)(now - lastDT.AddMinutes(addCount)).TotalSeconds / M_TO_S;
#else
                int count = (int)(now - lastDT.AddHours(addCount)).TotalSeconds / H_TO_S;
#endif

                if (count > 0)
                {
                    currency += RCRate * count;
                    addCount += count;
                }
            }
        }

        public static void CalcRemainTime()
        {
            var remainCurrency = MaxRC - currency;
            var remainCount = remainCurrency / RCRate;
            var lastDT = DateTime.Parse(lastInputTime, dtCulture);

            remainCount += (remainCurrency % RCRate) == 0 ? 0 : 1;

#if TEST
            endTime = lastDT.AddMinutes(addCount).AddMinutes(remainCount);
#else
            endTime = lastDT.AddHours(addCount).AddHours(remainCount);
#endif
        }

        public static void SaveValue()
        {
            try
            {
                Preferences.Set(SettingConstants.RC_COUNT, currency);
                Preferences.Set(SettingConstants.RC_ADD_COUNT, addCount);
                Preferences.Set(SettingConstants.RC_END_TIME, endTime.ToString(dtCulture));
                Preferences.Set(SettingConstants.RC_LAST_INPUT_TIME, lastInputTime);
                Preferences.Set(SettingConstants.RC_ADEPTAL_LEVEL, (int)realmRank);
                Preferences.Set(SettingConstants.RC_TRUST_RANK, trustRank);
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
        public enum GItemType { MagicCrystalChunk = 0, Artifact, Specialty, Artifact12H, CrystalChunk, WhiteIronChunk, IronChunk }

        public static double[] resetTimeList = { 72, 24, 48, 12, 72, 48, 24 };
    }

    public static class GadgetEnvironment
    {
        public enum GadgetType { ParametricTransformer = 0, PortableWaypoint }

        public static double[] resetTimeList = { 166, 168 };
    }

    public static class FurnishingEnvironment
    {
        public const int SPEEDUP_HOUR = 4;
        public enum FurnishType { Rarity2, Rarity3, Rarity4 }
        public static double[] resetTimeList = { 12, 14, 16 }; // Rarity : 2 3 4
    }

    public static class TalentEnvironment
    {
        public const int RENEWAL_HOUR = 4;

        public static Locations Location { get; set; }
        public static TalentItem Item { get; set; }

        public static void LoadSettings()
        {
            Server = (Servers)Preferences.Get(SettingConstants.APP_INGAMESERVER, 0);
            Location = (Locations)Preferences.Get(SettingConstants.ITEM_TALENT_LOCATION, 0);
        }        

        public static void CheckNowTalentBook()
        {
            int interval = TZInfo.BaseUtcOffset.Hours - serverUTCs[(int)Server];
            int realRenewalHour = RENEWAL_HOUR + interval;
            var now = DateTime.Now;

            var dowValue = (now.Hour - realRenewalHour) switch
            {
                int result when result < 0 => now.AddDays(-1).DayOfWeek,
                _ => now.DayOfWeek
            };

            Item = (from item in genshinDB.talentItems
                       where item.Location.Equals(Location) && item.AvailableDayOfWeeks.Contains(dowValue)
                       select item).First();
        }

        public static TalentItem CheckNowTalentBook(Locations location)
        {
            int interval = TZInfo.BaseUtcOffset.Hours - serverUTCs[(int)Server];
            int realRenewalHour = RENEWAL_HOUR + interval;
            var now = DateTime.Now;

            var dowValue = (now.Hour - realRenewalHour) switch
            {
                int result when result < 0 => now.AddDays(-1).DayOfWeek,
                _ => now.DayOfWeek
            };

            return (from item in genshinDB.talentItems
                    where item.Location.Equals(location) && item.AvailableDayOfWeeks.Contains(dowValue)
                    select item).First();
        }

        public static string GetTalentBookImageName()
        {
            return Item.ItemName switch
            {
                "Freedom" => "talent_freedom.png",
                "Resistance" => "talent_resistance.png",
                "Ballad" => "talent_ballad.png",
                "Prosperity" => "talent_prosperity.png",
                "Diligence" => "talent_diligence.png",
                "Gold" => "talent_gold.png",
                "All" => $"talent_all_{Location:F}.png",
                _ => ""
            };
        }
    }

    public static class WeaponAscensionEnvironment
    {
        public const int RENEWAL_HOUR = 4;

        public static Locations Location { get; set; }
        public static WeaponAscensionItem Item { get; set; }

        public static void LoadSettings()
        {
            Server = (Servers)Preferences.Get(SettingConstants.APP_INGAMESERVER, 0);
            Location = (Locations)Preferences.Get(SettingConstants.ITEM_WEAPONASCENSION_LOCATION, 0);
        }


        public static void CheckNowWAItem()
        {
            int interval = TZInfo.BaseUtcOffset.Hours - serverUTCs[(int)Server];
            int realRenewalHour = RENEWAL_HOUR + interval;
            var now = DateTime.Now;

            var dowValue = (now.Hour - realRenewalHour) switch
            {
                int result when result < 0 => now.AddDays(-1).DayOfWeek,
                _ => now.DayOfWeek
            };

            Item = (from item in genshinDB.weaponAscensionItems
                    where item.Location.Equals(Location) && item.AvailableDayOfWeeks.Contains(dowValue)
                    select item).First();
        }

        public static WeaponAscensionItem CheckNowWAItem(Locations location)
        {
            int interval = TZInfo.BaseUtcOffset.Hours - serverUTCs[(int)Server];
            int realRenewalHour = RENEWAL_HOUR + interval;
            var now = DateTime.Now;

            var dowValue = (now.Hour - realRenewalHour) switch
            {
                int result when result < 0 => now.AddDays(-1).DayOfWeek,
                _ => now.DayOfWeek
            };

            return (from item in genshinDB.weaponAscensionItems
                    where item.Location.Equals(location) && item.AvailableDayOfWeeks.Contains(dowValue)
                    select item).First();
        }

        public static string GetWPItemImageName()
        {
            return Item.ItemName switch
            {
                "Decarabian" => "wa_decarabian_4.png",
                "Boreal Wolf" => "wa_boreal_wolf_4.png",
                "The Dandelion Gladiator" => "wa_dandelion_gladiator_4.png",
                "Guyun" => "wa_guyun_4.png",
                "Mist Veiled" => "wa_mist_veiled_4.png",
                "Aerosiderite" => "wa_aerosiderite_4.png",
                "All" => $"wa_all_{Location:F}.png",
                _ => ""
            };
        }
    }

    public static class ChecklistEnvironment
    {
        public enum ResetType { Once = 0, Custom, Daily, Weekly, Monthly }
    }
}
