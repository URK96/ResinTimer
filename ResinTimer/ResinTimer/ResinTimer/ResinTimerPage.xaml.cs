using ResinTimer.Resources;

using System;
using System.Threading;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Timer = System.Timers.Timer;
using TTimer = System.Threading.Timer;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResinTimerPage : ContentPage
    {
        private Timer buttonPressTimer;
        private TTimer calcTimer;

        private int quickCalcValue;
        private int quickOTCalcValue;
        private bool isRunQuickCalc = false;
        private bool isQuickOTCalc = false;

        public ResinTimerPage()
        {
            InitializeComponent();

            ResinEnvironment.oneCountTime = new ResinTime(0);
            ResinEnvironment.totalCountTime = new ResinTime(0);

            ResinEnvironment.LoadValues();

            if (!(Device.RuntimePlatform == Device.UWP))
            {
                buttonPressTimer = new Timer(500)
                {
                    AutoReset = false
                };
                buttonPressTimer.Elapsed += delegate
                {
                    isRunQuickCalc = true;

                    if (Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true))
                    {
                        Vibration.Vibrate(100);
                    }

                    QuickCalc();
                };
            }

            calcTimer = new TTimer(CalcTimeResin, new AutoResetEvent(false), TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.5));
        }

        private void SetToolbar()
        {
            NotiToolbarItem.IsEnabled = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CautionResinOnlyLabel.IsVisible = ResinEnvironment.applyType == ResinEnvironment.ApplyType.Resin;

            calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.5));

            SetToolbar();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ResinEnvironment.SaveValue();

            calcTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var item = sender as ToolbarItem;

            switch (item.Text)
            {
                case "Edit":
                    await Navigation.PushAsync(new EditResinPage(), true);
                    break;
                case "Noti Setting":
                    await Navigation.PushAsync(new NotiSettingPage(), true);
                    break;
                default:
                    break;
            }
        }

        private void CalcTimeResin(object statusInfo)
        {
            try
            {
                if (ResinEnvironment.endTime.CompareTo(DateTime.Now) >= 0)
                {
                    (ResinEnvironment.totalCountTime, ResinEnvironment.oneCountTime) = ResinTime.CalcResinTime(ResinEnvironment.endTime);

                    ResinEnvironment.CalcResin();
                }
                else
                {
                    ResinEnvironment.totalCountTime.SetTime(0);
                    ResinEnvironment.oneCountTime.SetTime(0);
                    ResinEnvironment.resin = ResinEnvironment.MAX_RESIN;
                }

                MainThread.BeginInvokeOnMainThread(RefreshInfo);
            }
            catch { }
        }

        private void RefreshInfo()
        {
            try
            {
                TotalTimeHour.Text = $"{ResinEnvironment.totalCountTime.Hour:D2}";
                TotalTimeMinute.Text = $"{ResinEnvironment.totalCountTime.Min:D2}";

                LastInputDateTimeLabel.Text = ResinEnvironment.lastInputTime;
                EndDateTimeLabel.Text = $"{ResinEnvironment.endTime}";

                ResinCount.Text = ResinEnvironment.resin.ToString();
                OneCountTimer.Text = ResinEnvironment.oneCountTime.TimeMinSec;

                RangeValue.Value = ResinEnvironment.resin;
                PointerValue.Value = ResinEnvironment.resin;

                ResinRemainTimeRange.EndValue = 160 - ResinEnvironment.oneCountTime.TotalSec * ((double)ResinEnvironment.MAX_RESIN / ResinTime.ONE_RESTORE_INTERVAL);
            }
            catch (Exception) { }
        }

        private void QuickCalc()
        {
            var now = DateTime.Now;

            if (ResinEnvironment.endTime < now)
            {
                ResinEnvironment.endTime = now;
            }

            if (isQuickOTCalc)
            {
                ResinEnvironment.endTime = ResinEnvironment.endTime.AddSeconds(ResinTime.ONE_RESTORE_INTERVAL * (ResinEnvironment.resin / quickOTCalcValue) * quickOTCalcValue);

                isQuickOTCalc = false;
            }
            else
            {
                ResinEnvironment.endTime = ((ResinEnvironment.resin - quickCalcValue) < 0) ?
                    now.AddSeconds(ResinTime.ONE_RESTORE_INTERVAL * ResinEnvironment.MAX_RESIN) :
                    ResinEnvironment.endTime.AddSeconds(ResinTime.ONE_RESTORE_INTERVAL * quickCalcValue);
            }

            ResinEnvironment.lastInputTime = now.ToString();

            ResinEnvironment.CalcResin();
            ResinEnvironment.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                var notiManager = new ResinNotiManager();
                notiManager.UpdateNotisTime();
            }
        }

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");
                await button.ScaleTo(0.95, 100, Easing.SinInOut);

                if (!int.TryParse(button.Text, out quickCalcValue))
                {
                    quickOTCalcValue = int.Parse(button.Text.Substring(2));

                    isQuickOTCalc = true;
                }
                else
                {
                    quickCalcValue = -quickCalcValue;
                }
                
                isRunQuickCalc = false;

                if (Device.RuntimePlatform == Device.UWP)
                {
                    QuickCalc();
                }
                else
                {
                    buttonPressTimer.Start();
                }
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

                if (!(Device.RuntimePlatform == Device.UWP))
                {
                    buttonPressTimer.Stop();

                    if (!isRunQuickCalc)
                    {
                        DependencyService.Get<IToast>().Show(AppResources.MainPage_QuickCalcButton_Toast);
                    }
                }
            }
            catch { }
        }

    }
}