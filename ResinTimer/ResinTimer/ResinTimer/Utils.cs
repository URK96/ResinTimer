using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Essentials;
using Xamarin.Forms;

using static ResinTimer.AppEnvironment;

namespace ResinTimer
{
    public static class Utils
    {
        public static string UID => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_UID, string.Empty);
        public static string Ltuid => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTUID, string.Empty);
        public static string Ltoken => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTOKEN, string.Empty);

        public static void ResetAccountInfo()
        {
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_UID, string.Empty);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTUID, string.Empty);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTOKEN, string.Empty);
        }

        public static void SetAccountCookieInfo(string ltuid, string ltoken)
        {
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTUID, ltuid);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTOKEN, ltoken);
        }

        public static void ShowToast(string message)
        {
            DependencyService.Get<IToast>().Show(message);
        }

        public static string GetTimeString(DateTime dt)
        {
            var langValue = (AppLang)Preferences.Get(SettingConstants.APP_LANG, (int)AppLang.System);
            bool setting24H = Preferences.Get(SettingConstants.APP_USE_24H_TIMEFORMAT, false);
            string date = $"{dt:d}";
            string time = dt.ToString($"{(setting24H ? "HH" : "hh")}:mm:ss");

            string timeString;

            switch (langValue)
            {
                case AppLang.Korean:
                case AppLang.System when CultureInfo.InstalledUICulture.Name.Equals("ko-KR"):
                    timeString = $"{date} {(setting24H ? string.Empty : $"{dt:tt} ")}{time}";
                    break;
                default:
                    timeString = $"{date} {time}{(setting24H ? string.Empty : $" {dt:tt}")}";
                    break;
            }

            return timeString;
        }
    }
}
