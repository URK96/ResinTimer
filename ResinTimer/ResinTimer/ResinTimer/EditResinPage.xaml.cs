using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ResinTimer.Resources;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditResinPage : TabbedPage
    {
        public EditResinPage()
        {
            InitializeComponent();
        }

        private void SetTimeValue()
        {
            int.TryParse(NowTotalTimeHour.Text, out int hour);
            int.TryParse(NowTotalTimeMinute.Text, out int min);
            int.TryParse(NowTotalTimeSecond.Text, out int sec);

            ResinEnvironment.totalCountTime.SetTime(hour, min, sec);
        }

        private void SetResinValue()
        {
            int.TryParse(NowResinCount.Text, out ResinEnvironment.resin);
        }

        private void CalcRemainTime()
        {
            var now = DateTime.Now;

            switch (ResinEnvironment.applyType)
            {
                case ResinEnvironment.ApplyType.Time:
                    int totalSec = ResinEnvironment.totalCountTime.TotalSec;

                    ResinEnvironment.oneCountTime.SetTime(totalSec % ResinTime.ONE_RESTORE_INTERVAL);
                    break;
                case ResinEnvironment.ApplyType.Resin:
                    ResinEnvironment.oneCountTime.SetTime(ResinTime.ONE_RESTORE_INTERVAL);
                    ResinEnvironment.totalCountTime.SetTime(ResinTime.ONE_RESTORE_INTERVAL * (ResinEnvironment.MAX_RESIN - ResinEnvironment.resin));
                    break;
            }

            ResinEnvironment.endTime = now.AddHours(ResinEnvironment.totalCountTime.Hour).AddMinutes(ResinEnvironment.totalCountTime.Min).AddSeconds(ResinEnvironment.totalCountTime.Sec);
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            if (CurrentPage.Title == AppResources.EditPage_TabTime_Title)
            {
                ResinEnvironment.applyType = ResinEnvironment.ApplyType.Time;
            }
            else if (CurrentPage.Title == AppResources.EditPage_TabResin_Title)
            {
                ResinEnvironment.applyType = ResinEnvironment.ApplyType.Resin;
            }

            switch (ResinEnvironment.applyType)
            {
                case ResinEnvironment.ApplyType.Time:
                    SetTimeValue();
                    break;
                case ResinEnvironment.ApplyType.Resin:
                    SetResinValue();
                    break;
                default:
                    break;
            }

            CalcRemainTime();
            ResinEnvironment.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                var notiManager = new ResinNotiManager();
                notiManager.UpdateNotisTime();
            }

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
    }
}