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
    public partial class EditPage : ContentPage
    {
        public EditPage()
        {
            InitializeComponent();
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            SetValue();
            CalcRemainTimeResin();
            SaveValue();

            Navigation.PopAsync();
        }

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");
                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void ButtonReleased(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.Transparent;
                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }

        private void SetValue()
        {
            int hour, min, sec;

            int.TryParse(NowTotalTimeHour.Text, out hour);
            int.TryParse(NowTotalTimeMinute.Text, out min);
            int.TryParse(NowTotalTimeSecond.Text, out sec);

            ResinEnvironment.totalCountTime.SetTime(hour, min, sec);
        }

        private void CalcRemainTimeResin()
        {
            var now = DateTime.Now;
            int totalSec = ResinEnvironment.totalCountTime.TotalSec;

            ResinEnvironment.oneCountTime.SetTime(totalSec % ResinTime.ONE_RESTORE_INTERVAL);
            ResinEnvironment.endTime = now.AddHours(ResinEnvironment.totalCountTime.Hour).AddMinutes(ResinEnvironment.totalCountTime.Min).AddSeconds(ResinEnvironment.totalCountTime.Sec);
        }

        private void SaveValue()
        {
            Preferences.Set(SettingConstants.RESIN_COUNT, ResinEnvironment.resin);
            Preferences.Set(SettingConstants.END_TIME, ResinEnvironment.endTime.ToString());
        }
    }
}