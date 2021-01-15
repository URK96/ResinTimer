using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Timer = System.Timers.Timer;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResinTimerPage : ContentPage
    {
        private Mutex mutex;

        private Thread calcThread;

        private Timer buttonPressTimer;

        private int quickCalcValue;
        private int quickOTCalcValue;
        private bool isRunQuickCalc = false;
        private bool isQuickOTCalc = false;

        public ResinTimerPage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Title = string.Empty;
            }

            //var animation = new Animation
            //{
            //    { 0, 0.5, new Animation(v => TimeSeperator.Opacity = v, 1, 0) },
            //    { 0.5, 1, new Animation(v => TimeSeperator.Opacity = v, 0, 1) }
            //};
            //animation.Commit(this, "TimeSeperatorAnimation", 16, 2000, Easing.BounceIn, repeat: () => true);

            ResinEnvironment.oneCountTime = new ResinTime(0);
            ResinEnvironment.totalCountTime = new ResinTime(0);

            ResinEnvironment.LoadValues();

            mutex = new Mutex(false);

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

            calcThread = new Thread(new ThreadStart(CalcTimeResin))
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            calcThread.Start();
        }

        private void SetToolbar()
        {
            NotiToolbarItem.IsEnabled = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CautionResinOnlyLabel.IsVisible = ResinEnvironment.applyType == ResinEnvironment.ApplyType.Resin;

            try
            {
                mutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                // DependencyService.Get<IToast>().Show(ex.ToString());
            }

            SetToolbar();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ResinEnvironment.SaveValue();

            mutex.WaitOne();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {

            }
        }

        private void CalcTimeResin()
        {
            while (true)
            {
                try
                {
                    mutex.WaitOne();

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
                catch (Exception ex)
                {

                }
                finally
                {
                    mutex.ReleaseMutex();

                    Thread.Sleep(500);
                }
            }
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
            catch (Exception ex)
            {

            }
        }

        private void QuickCalc()
        {
            mutex.WaitOne();

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

            mutex.ReleaseMutex();
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