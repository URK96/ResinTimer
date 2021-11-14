using GenshinDB_Core;

using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

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


        public static CultureInfo dtCulture = new CultureInfo("en-US");
        public static bool isDebug = false;
        public static bool isRunningNotiThread = false;

        public static GenshinDB genshinDB;

        public static Servers Server { get; set; }
        public static TimeZoneInfo TZInfo { get; set; }

        public static int[] serverUTCs = { -5, 8, 1, 8 };
        public static string[] serverList = new string[] { "America", "Asia", "Europe", "TW, HK, MO" };

        public static List<string> locations;

        public static void LoadNowTZInfo() => TZInfo = TimeZoneInfo.Local;

        public static void LoadLocationList() => locations = genshinDB.GetAllLocations();
        public static string GetUTCString(int offset) => $"UTC{((offset >= 0) ? "+" : "")}{offset}";

        public static Color GetBackgroundColor()
        {
            if (Device.RuntimePlatform == Device.iOS)
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
                (int)AppLang.English => CultureInfo.GetCultureInfo("en-US"),
                (int)AppLang.Korean => CultureInfo.GetCultureInfo("ko-KR"),
                _ when CultureInfo.CurrentUICulture.Name.Equals("ko-KR") => CultureInfo.GetCultureInfo("ko-KR"),
                _ => CultureInfo.GetCultureInfo("en-US")
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
