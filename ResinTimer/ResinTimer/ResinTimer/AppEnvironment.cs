﻿using GenshinDB_Core;

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
            TimerExpedition,
            TimerGatheringItem,
            TimerGadget,
            TimerTalentBook,
            WidgetResin,
            WidgetTalentBook
        }

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

        public static async void RefreshCollectionView<T>(CollectionView cv, List<T> list)
        {
            ResetCollectionViewSelection(cv);

            cv.ItemsSource = Array.Empty<T>();
            await Task.Delay(10);
            cv.ItemsSource = list;
        }

        public static void ResetCollectionViewSelection(CollectionView cv)
        {
            cv.SelectedItems = null;
            cv.SelectedItem = null;
        }
    }
}
