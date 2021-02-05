using System;

using Xamarin.Essentials;

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
            if (Preferences.ContainsKey(SettingConstants.END_TIME))
            {
                string endTimeP = Preferences.Get(SettingConstants.END_TIME, "");

                if (!DateTime.TryParse(endTimeP, out endTime))
                {
                    endTime = DateTime.Now;
                }
            }
            else
            {
                var now = DateTime.Now;

                Preferences.Set(SettingConstants.END_TIME, now.ToString());
                endTime = now;
            }

            lastInputTime = Preferences.Get(SettingConstants.LAST_INPUT_TIME, DateTime.Now.ToString());
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
                Preferences.Set(SettingConstants.END_TIME, endTime.ToString());
                Preferences.Set(SettingConstants.LAST_INPUT_TIME, lastInputTime);
                Preferences.Set(SettingConstants.RESIN_INPUT_TYPE, (int)applyType);
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

        public static double[] ResetTimeList = { 72, 24, 48, 12, 72, 48, 24 };
    }

    public static class TalentEnvironment
    {
        public const int RENEWAL_HOUR = 4;

        public enum Servers { America, Asia, Europe, TWHKMO }

        public static int[] serverUTCs = { -5, 8, 1, 8 };

        public static string[] serverList = new string[] { "America", "Asia", "Europe", "TW, HK, MO" };
    }
}
