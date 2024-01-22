using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using GenshinInfo.Managers;

using Xamarin.Essentials;
using Xamarin.Forms;

using static ResinTimer.AppEnvironment;

namespace ResinTimer
{
    public static class Utils
    {
        public static string UID => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_UID, string.Empty);
        public static string Ltuid
        {
            get
            {
                string ltuid = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTUID_V2, string.Empty);

                if (string.IsNullOrWhiteSpace(ltuid))
                {
                    ltuid = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTUID, string.Empty);
                }

                return ltuid;
            }
        }
        public static string Ltoken
        {
            get
            {
                string ltoken = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTOKEN_V2, string.Empty);

                if (string.IsNullOrWhiteSpace(ltoken))
                {
                    ltoken = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTOKEN, string.Empty);
                }

                return ltoken;
            }
        }

        public static bool UseV2CookieInfo => !string.IsNullOrWhiteSpace(
            Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_LTUID_V2, string.Empty));

        public static Command<string> UrlOpenCommand => new(async (url) => await Launcher.OpenAsync(url));

        public static void ResetAccountInfo()
        {
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_UID, string.Empty);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTUID, string.Empty);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTOKEN, string.Empty);
        }

        public static void SetAccountCookieInfo(string ltuid, string ltoken)
        {
            ResetAccountV2CookieInfo();

            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTUID, ltuid);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTOKEN, ltoken);
        }

        public static void ResetAccountCookieInfo()
        {
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTUID, string.Empty);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTOKEN, string.Empty);
        }

        public static void SetAccountV2CookieInfo(string ltuid, string ltoken)
        {
            ResetAccountCookieInfo();

            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTUID_V2, ltuid);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTOKEN_V2, ltoken);
        }

        public static void ResetAccountV2CookieInfo()
        {
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTUID_V2, string.Empty);
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_LTOKEN_V2, string.Empty);
        }

        public static GenshinInfoManager CreateGenshinInfoManagerInstance() =>
            new(UID, Ltuid, Ltoken)
            {
                UseV2Cookie = UseV2CookieInfo
            };

        public static void ShowToast(string message)
        {
            DependencyService.Get<IToast>().Show(message);
        }

        /// <summary>
        /// Convert specific date/time string to DateTime
        /// </summary>
        /// <param name="dateTimeStr">Input 'yyyy,MM,dd,HH,mm,ss' format date/time string</param>
        /// <returns>DateTime result from input string</returns>
        public static DateTime GetDateTimeFromString(string dateTimeStr)
        {
            string[] splits = dateTimeStr.Split(',');

            DateTime dt = new(
                int.Parse(splits[0]),
                int.Parse(splits[1]),
                int.Parse(splits[2]),
                int.Parse(splits[3]),
                int.Parse(splits[4]),
                int.Parse(splits[5]));

            return dt;
        }

        public static string GetTimeString(DateTime dt)
        {
            var langValue = (AppLang)Preferences.Get(SettingConstants.APP_LANG, (int)AppLang.System);
            bool setting24H = Preferences.Get(SettingConstants.APP_USE_24H_TIMEFORMAT, false);
            string date = $"{dt:d}";
            string time = dt.ToString($"{(setting24H ? "HH" : "hh")}:mm:ss");

            return langValue switch
            {
                AppLang.Korean or
                AppLang.System when CultureInfo.CurrentUICulture.Name.Equals("ko-KR") =>
                    $"{date} {(setting24H ? string.Empty : $"{dt:tt} ")}{time}",

                _ => $"{date} {time}{(setting24H ? string.Empty : $" {dt:tt}")}"
            };
        }

        public static DayOfWeek GetServerDayOfWeek()
        {
            const int RenewalHour = 4;

            LoadAppSettings();

            int interval = TZInfo.BaseUtcOffset.Hours - ServerUTCs[(int)Server];
            int realRenewalHour = RenewalHour + interval;
            DateTime now = DateTime.Now;

            return (now.Hour - realRenewalHour) switch
            {
                int result when result < 0 => now.AddDays(-1).DayOfWeek,
                _ => now.DayOfWeek
            };
        }

        public static void RefreshCollectionView<T>(CollectionView cv, List<T> list)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                cv.ItemsSource = Array.Empty<T>();

                await Task.Delay(10);

                cv.ItemsSource = list;

                ResetCollectionViewSelection(cv);
            });
        }

        public static void ResetCollectionViewSelection(CollectionView cv)
        {
            cv.SelectedItems = null;
            cv.SelectedItem = null;
        }
    }
}
