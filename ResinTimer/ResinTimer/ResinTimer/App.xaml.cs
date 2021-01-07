using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using ResinTimer.Resources;

using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace ResinTimer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            AppResources.Culture = CultureInfo.InstalledUICulture;

#if DEBUG
            AppEnvironment.isDebug = true;
#endif

            SetDefaultPreferences();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzY5MzQ5QDMxMzgyZTM0MmUzMGx5ajVXODdXRldsSjMvRFNnbkVxS2c2ZTJkdEhxNW4yQVlLSCtsbWt1WG89");

            MainPage = new MainPage();
        }

        private void SetDefaultPreferences()
        {
            if (!Preferences.ContainsKey(SettingConstants.QUICKCALC_VIBRATION))
            {
                Preferences.Set(SettingConstants.QUICKCALC_VIBRATION, true);
            }
            if (!Preferences.ContainsKey(SettingConstants.NOTI_ENABLED))
            {
                Preferences.Set(SettingConstants.NOTI_ENABLED, false);
            }
            if (!Preferences.ContainsKey(SettingConstants.NOTI_LIST))
            {
                var list = new List<ResinNoti>
                {
                    new ResinNoti(ResinEnvironment.MAX_RESIN)
                };

                Preferences.Set(SettingConstants.NOTI_LIST, JsonConvert.SerializeObject(list));
            }
        }

        protected override void OnStart()
        {
            var sb = new StringBuilder();
            sb.Append("android=4c940536-5e25-4e22-a445-1f5ae7cc254d;");
            sb.Append("uwp=4a09233c-0d70-4ae8-9987-1beed7439a61;");
            sb.Append("ios=8e875b1a-243a-4293-9d81-7b19c4fe59f5");

            AppCenter.Start(sb.ToString(), typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
