using ResinTimer.Resources;

using System;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

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

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzU1Mjk3QDMxMzgyZTMzMmUzMGloVERVWXNDOTdjSUF2UU91TWk4b3R1TUQ5YUI0bXhEcVRGYXJDQjRhYWM9");

            MainPage = new NavigationPage(new MainPage());
        }

        private void SetDefaultPreferences()
        {
            if (!Preferences.ContainsKey(SettingConstants.QUICKCALC_VIBRATION))
            {
                Preferences.Set(SettingConstants.QUICKCALC_VIBRATION, true);
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
