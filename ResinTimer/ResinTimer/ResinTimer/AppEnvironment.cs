using GenshinDB_Core;

using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Globalization;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace ResinTimer
{
    public static class AppEnvironment
    {
        public enum AppLang { System = 0, English, Korean }
        public enum Servers { America, Asia, Europe, TWHKMO }
        public enum ManualCategory
        {
            TimerResin = 0,
            TimerRealmCurrency,
            TimerExpedition,
            TimerGatheringItem,
            TimerGadget,
            TimerFurnishing,
            TimerTalentBook,
            TimerWeaponAscension,
            WidgetResin,
            WidgetTalentBook,
            WidgetWeaponAscension
        }
        public enum TimerPage
        {
            Resin = 0,
            RealmCurrency = 1,
            RealmFriendship,
            Expedition,
            GatheringItem,
            Gadget,
            Furnishing,
            Gardening,
            Talent,
            WeaponAscension
        }


        public static CultureInfo dtCulture = new("en-US");
        public static bool isDebug = false;
        public static bool isRunningNotiThread = false;

        public static GenshinDB genshinDB;

        public static Servers Server { get; set; }
        public static TimeZoneInfo TZInfo => TimeZoneInfo.Local;

        public static int[] serverUTCs = { -5, 8, 1, 8 };
        public static string[] serverList = new string[] { "America", "Asia", "Europe", "TW, HK, MO" };

        public static List<string> locations;

        public static string GetLangShortCode => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        public static void LoadLocationList() => locations = genshinDB.GetAllLocations();
        public static string GetUTCString(int offset) => $"UTC{((offset >= 0) ? "+" : "")}{offset}";

        public static Color GetBackgroundColor()
        {
            if (Device.RuntimePlatform is Device.iOS)
            {
                return (Application.Current.RequestedTheme == OSAppTheme.Dark) ? Color.Black : Color.White;
            }
            else
            {
                return Color.Default;
            }
        }

        public static void InitAppLang()
        {
            int settingValue = Preferences.Get(SettingConstants.APP_LANG, (int)AppLang.System);

            AppResources.Culture = CultureInfo.CurrentCulture = settingValue switch
            {
                (int)AppLang.English or
                (int)AppLang.System when CultureInfo.CurrentUICulture.Name.Contains("en") => CultureInfo.GetCultureInfo("en-US"),
                (int)AppLang.Korean or
                (int)AppLang.System when CultureInfo.CurrentUICulture.Name.Contains("ko") => CultureInfo.GetCultureInfo("ko-KR"),
                _ => CultureInfo.GetCultureInfo("en-US")
            };
        }

        public static void LoadAppSettings()
        {
            Server = (Servers)Preferences.Get(SettingConstants.APP_INGAMESERVER, 0);
        }
    }
}
