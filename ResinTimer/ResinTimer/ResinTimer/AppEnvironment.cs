using GenshinDB_Core;

using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace ResinTimer
{
    public static class AppEnvironment
    {
        public enum StartPageEnum
        {
            Default = 0,
            TalentBookCharacter = 1
        }
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


        public static CultureInfo DTCulture = new("en-US");
        public static bool IsDebug = false;
        public static bool IsRunningNotiThread = false;

        public static GenshinDB GDB;

        public static Servers Server { get; set; }
        public static TimeZoneInfo TZInfo => TimeZoneInfo.Local;

        public static int[] ServerUTCs = { -5, 8, 1, 8 };
        public static string[] ServerList = new string[] { "America", "Asia", "Europe", "TW, HK, MO" };

        public static List<string> Locations;

        public static string GetLangShortCode => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        public static void LoadLocationList() => Locations = GDB.GetAllLocations();
        public static string GetUTCString(int offset) => $"UTC{((offset >= 0) ? "+" : "")}{offset}";

        public static Color GetBackgroundColor()
        {
            if (Device.RuntimePlatform is Device.iOS)
            {
                return (Application.Current.RequestedTheme is OSAppTheme.Dark) ? Color.Black : Color.White;
            }
            else
            {
                return Color.Default;
            }
        }

        public static void InitAppLang()
        {
            int settingValue = Preferences.Get(SettingConstants.APP_LANG, (int)AppLang.System);

            Thread.CurrentThread.CurrentUICulture = AppResources.Culture = CultureInfo.CurrentCulture = 
                settingValue switch
            {
                (int)AppLang.Korean => CultureInfo.GetCultureInfo("ko-KR"),
                (int)AppLang.System when CultureInfo.CurrentUICulture.Name.Contains("ko") => 
                    CultureInfo.GetCultureInfo("ko-KR"),

                _ => CultureInfo.GetCultureInfo("en-US")
            };
        }

        public static void LoadAppSettings()
        {
            Server = (Servers)Preferences.Get(SettingConstants.APP_INGAMESERVER, 0);
        }
    }
}
