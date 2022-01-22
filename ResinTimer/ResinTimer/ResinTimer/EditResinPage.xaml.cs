using ResinTimer.Managers.NotiManagers;
using ResinTimer.Resources;

using System;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppEnv = ResinTimer.AppEnvironment;
using REnv = ResinTimer.ResinEnvironment;

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

            REnv.TotalCountTime = new(hour, min, sec);
        }

        private void SetResinValue()
        {
            int.TryParse(NowResinCount.Text, out REnv.Resin);
        }

        private void CalcRemainTime()
        {
            DateTime now = DateTime.Now;

            switch (REnv.ManualApplyType)
            {
                case REnv.ApplyType.Time:
                    REnv.OneCountTime = TimeSpan.FromSeconds(
                        REnv.TotalCountTime.TotalSeconds % REnv.OneRestoreInterval);
                    break;
                case REnv.ApplyType.Resin:
                    REnv.OneCountTime = TimeSpan.FromSeconds(REnv.OneRestoreInterval);
                    REnv.TotalCountTime = TimeSpan.FromSeconds(
                        REnv.OneRestoreInterval * (REnv.MaxResin - REnv.Resin));
                    break;
            }

            REnv.EndTime = now.AddSeconds(REnv.TotalCountTime.TotalSeconds);
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            if (CurrentPage.Title == AppResources.EditPage_TabTime_Title)
            {
                REnv.ManualApplyType = REnv.ApplyType.Time;
            }
            else if (CurrentPage.Title == AppResources.EditPage_TabResin_Title)
            {
                REnv.ManualApplyType = REnv.ApplyType.Resin;
            }

            switch (REnv.ManualApplyType)
            {
                case REnv.ApplyType.Time:
                    SetTimeValue();
                    break;
                case REnv.ApplyType.Resin:
                    SetResinValue();
                    break;
                default:
                    break;
            }

            REnv.LastInputTime = DateTime.Now.ToString(AppEnv.DTCulture);

            CalcRemainTime();
            REnv.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                ResinNotiManager notiManager = new();

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