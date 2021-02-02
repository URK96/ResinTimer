﻿
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

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

            _ = CreateAppAction();
            AppActions.OnAppAction += AppActions_OnAppAction;

#if DEBUG
            AppEnvironment.isDebug = true;
#endif
            AppEnvironment.genshinDB = new GenshinDB_Core.GenshinDB();

            SetDefaultPreferences();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzY5MzQ5QDMxMzgyZTM0MmUzMGx5ajVXODdXRldsSjMvRFNnbkVxS2c2ZTJkdEhxNW4yQVlLSCtsbWt1WG89");

            MainPage = new MainPage();
        }

        private void SetDefaultPreferences()
        {
            if (!Preferences.ContainsKey(SettingConstants.LAST_INPUT_TIME))
            {
                Preferences.Set(SettingConstants.LAST_INPUT_TIME, DateTime.Now.ToString());
            }
            if (!Preferences.ContainsKey(SettingConstants.RESIN_INPUT_TYPE))
            {
                Preferences.Set(SettingConstants.RESIN_INPUT_TYPE, (int)ResinEnvironment.ApplyType.Time);
            }
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

        private async Task CreateAppAction()
        {
            try
            {
                await AppActions.SetAsync(
                    new AppAction("app_timer_resin", AppResources.AppAction_App_Timer_Resin, icon: "resin.png"),
                    new AppAction("app_timer_expedition", AppResources.AppAction_App_Timer_Expedition, icon: "compass.png"),
                    new AppAction("app_timer_gatheringitem", AppResources.AppAction_App_Timer_GatheringItem, icon: "silk_flower.png"),
                    new AppAction("app_timer_talent", AppResources.AppAction_App_Timer_Talent, icon: "talent_freedom.png"));
            }
            catch (Exception ex)
            {

            }
        }

        private void AppActions_OnAppAction(object sender, AppActionEventArgs e)
        {
            if ((Current != this) && (Current is App app))
            {
                AppActions.OnAppAction -= app.AppActions_OnAppAction;

                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                MainPage = new MainPage(e.AppAction.Id);
                //await Current.MainPage.Navigation.PopToRootAsync();
                //await Current.MainPage.Navigation.PushAsync(new MainPage(e.AppAction.Id));
            });
        }

        protected override void OnStart()
        {
#if DEBUG

#else
            var sb = new StringBuilder();
            sb.Append("android=4c940536-5e25-4e22-a445-1f5ae7cc254d;");
            sb.Append("uwp=4a09233c-0d70-4ae8-9987-1beed7439a61;");
            sb.Append("ios=8e875b1a-243a-4293-9d81-7b19c4fe59f5");

            AppCenter.Start(sb.ToString(), typeof(Analytics), typeof(Crashes));
#endif
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
