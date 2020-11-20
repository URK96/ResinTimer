using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Essentials;

namespace ResinTimer
{
    public static class ResinEnvironment
    {
        public const int MAX_RESIN = 160;

        public static DateTime endTime;
        public static ResinTime totalCountTime;
        public static ResinTime oneCountTime;

        public static int resin = 0;

        public static void LoadValues()
        {
            if (Preferences.ContainsKey(SettingConstants.END_TIME))
            {
                string endTimeP = Preferences.Get(SettingConstants.END_TIME, "");
                endTime = string.IsNullOrWhiteSpace(endTimeP) ? DateTime.Now : DateTime.Parse(endTimeP);
            }
            else
            {
                var now = DateTime.Now;

                Preferences.Set(SettingConstants.END_TIME, now.ToString());
                endTime = now;
            }

            if (Preferences.ContainsKey(SettingConstants.RESIN_COUNT))
            {
                resin = Preferences.Get(SettingConstants.RESIN_COUNT, 0);
            }
            else
            {
                resin = 0;
                Preferences.Set(SettingConstants.RESIN_COUNT, resin);
            }

            //ResinEnvironment.endTime = DateTime.Now.AddSeconds(10).AddMinutes(8);
            //Resin = 40;
        }

        public static void CalcResin()
        {
            resin = 160 - (Convert.ToInt32((endTime - DateTime.Now).TotalSeconds) / ResinTime.ONE_RESTORE_INTERVAL) - 1;
        }
    }
}
