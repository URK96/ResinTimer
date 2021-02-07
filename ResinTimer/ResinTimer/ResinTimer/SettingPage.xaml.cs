﻿using ResinTimer.Dialogs;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        string[] StartScreenList => new string[]
        {
            AppResources.MasterDetail_MasterList_Resin,
            AppResources.MasterDetail_MasterList_Expedition,
            AppResources.MasterDetail_MasterList_GatheringItem,
            AppResources.MasterDetail_MasterList_Talent
        };

        public SettingPage()
        {
            InitializeComponent();

            LoadSettingValue();
        }

        private void LoadSettingValue()
        {
            // App Section
            Notification.On = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
            StartDetailScreenNow.Text = GetStartScreenString(Preferences.Get(SettingConstants.APP_START_DETAILSCREEN, 0));

            // Main Section
            QuickCalcVibration.IsEnabled = Device.RuntimePlatform != Device.UWP;
            QuickCalcVibration.On = Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true);
        }

        private string GetStartScreenString(int value)
        {
            return value switch
            {
                1 => AppResources.MasterDetail_MasterList_Expedition,
                2 => AppResources.MasterDetail_MasterList_GatheringItem,
                3 => AppResources.MasterDetail_MasterList_Talent,
                _ => AppResources.MasterDetail_MasterList_Resin
            };
        }

        private void QuickCalcVibration_OnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.QUICKCALC_VIBRATION, e.Value);
        }

        private async void Notification_OnChanged(object sender, ToggledEventArgs e)
        {
            //if (Device.RuntimePlatform == Device.iOS)
            //{
            //    var scheduledService = DependencyService.Get<IScheduledNoti>();

            //    if (e.Value)
            //    {
            //        scheduledService.TestNoti();
            //    }
            //    else
            //    {
            //        scheduledService.CancelAll();
            //    }

            //    return;
            //}

            var bootService = DependencyService.Get<IBootService>();

            Preferences.Set(SettingConstants.NOTI_ENABLED, e.Value);

            if (e.Value)
            {
                if (Device.RuntimePlatform == Device.UWP)
                {
                    if (!await bootService.Register())
                    {
                        string title = AppResources.Bootstrap_ChangeEnableFail_Title;
                        string message = AppResources.Bootstrap_ChangeEnableFail_Message;
                        string ok = AppResources.Dialog_Ok;

                        await DisplayAlert(title, message, ok);
                    }
                }

                var resinNotiManager = new ResinNotiManager();
                resinNotiManager.UpdateNotisTime();

                var expNotiManager = new ExpeditionNotiManager();
                expNotiManager.UpdateScheduledNoti<ExpeditionNoti>();

                var giNotiManager = new GatheringItemNotiManager();
                giNotiManager.UpdateScheduledNoti<GatheringItemNoti>();
            }
            else
            {
                DependencyService.Get<IScheduledNoti>().CancelAll();

                if (Device.RuntimePlatform == Device.UWP)
                {
                    await bootService.Unregister();
                }
            }
        }

        private async void StartDetailScreen_Tapped(object sender, EventArgs e)
        {
            var dialog = new BaseDialog(AppResources.SettingPage_Section_App_Start_DetailScreen_Dialog_Title,
                new RadioPreferenceView(StartScreenList, SettingConstants.APP_START_DETAILSCREEN));

            dialog.OnClose += delegate { StartDetailScreenNow.Text = GetStartScreenString(Preferences.Get(SettingConstants.APP_START_DETAILSCREEN, 0)); };

            await PopupNavigation.Instance.PushAsync(dialog);
        }
    }
}