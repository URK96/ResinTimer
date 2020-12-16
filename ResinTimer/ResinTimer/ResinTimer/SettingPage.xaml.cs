using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

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
            Notification.IsEnabled = Device.RuntimePlatform != Device.UWP;
            Notification.On = Preferences.Get(SettingConstants.NOTI_ENABLED, false);

            // Main Section
            QuickCalcVibration.IsEnabled = Device.RuntimePlatform != Device.UWP;
            QuickCalcVibration.On = Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true);
        }

        private void QuickCalcVibration_OnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.QUICKCALC_VIBRATION, e.Value);
        }

        private void Notification_OnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.NOTI_ENABLED, e.Value);

            if (e.Value)
            {
                var notiManager = new NotiManager();
                notiManager.UpdateNotisTime();
            }
            else
            {
                DependencyService.Get<IScheduledNoti>().CancelAll();
            }
        }
    }
}