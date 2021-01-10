using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();

            LoadSettingValue();
        }

        private void LoadSettingValue()
        {
            // App Section
            Notification.On = Preferences.Get(SettingConstants.NOTI_ENABLED, false);

            // Main Section
            QuickCalcVibration.IsEnabled = Device.RuntimePlatform != Device.UWP;
            QuickCalcVibration.On = Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true);
        }

        private void QuickCalcVibration_OnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.QUICKCALC_VIBRATION, e.Value);
        }

        private async void Notification_OnChanged(object sender, ToggledEventArgs e)
        {
            var bootService = DependencyService.Get<IBootService>();

            Preferences.Set(SettingConstants.NOTI_ENABLED, e.Value);

            if (e.Value)
            {
                if (Device.RuntimePlatform == Device.UWP)
                {
                    if (!await bootService.Register())
                    {
                        string title = ResinTimer.Resources.AppResources.Bootstrap_ChangeEnableFail_Title;
                        string message = ResinTimer.Resources.AppResources.Bootstrap_ChangeEnableFail_Message;
                        string ok = ResinTimer.Resources.AppResources.Dialog_Ok;

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
    }
}