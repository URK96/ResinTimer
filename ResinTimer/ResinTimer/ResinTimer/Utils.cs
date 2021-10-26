using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Essentials;
using Xamarin.Forms;

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
    }
}
