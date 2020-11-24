using ResinTimer.Resources;

using System;
using System.Threading;
using System.Timers;

using Xamarin.Essentials;
using Xamarin.Forms;

using Timer = System.Timers.Timer;

namespace ResinTimer
{
    public partial class MainPage : ContentPage
    {
        private Mutex mutex;

        private Thread calcThread;

        private Timer quickCalcTimer;

        private int quickCalcValue;
        private bool isRunQuickCalc = false;

        public MainPage()
        {
            InitializeComponent();

            var animation = new Animation
            {
                { 0, 0.5, new Animation(v => TimeSeperator.Opacity = v, 1, 0) },
                { 0.5, 1, new Animation(v => TimeSeperator.Opacity = v, 0, 1) }
            };
            animation.Commit(this, "TimeSeperatorAnimation", 16, 2000, Easing.BounceIn, repeat: () => true);

            ResinEnvironment.oneCountTime = new ResinTime(0);
            ResinEnvironment.totalCountTime = new ResinTime(0);

            ResinEnvironment.LoadValues();

            mutex = new Mutex(false);

            if (!(Device.RuntimePlatform == Device.UWP))
            {
                quickCalcTimer = new Timer(500)
                {
                    AutoReset = false
                };
                quickCalcTimer.Elapsed += delegate
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
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Preferences.Set(SettingConstants.END_TIME, ResinEnvironment.endTime.ToString());
            Preferences.Set(SettingConstants.RESIN_COUNT, ResinEnvironment.resin);

            mutex.WaitOne();
        }

        private void EditToolbarItemClicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new EditPage(), true);
            }
            catch { }
        }
        
        private void InfoToolbarItemClicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new InfoPage(), true);
            }
            catch { }
        }

        private void SettingToolbarItemClicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new SettingPage(), true);
            }
            catch { }
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

            ResinEnvironment.endTime = ResinEnvironment.endTime.AddSeconds(ResinTime.ONE_RESTORE_INTERVAL * quickCalcValue);

            mutex.ReleaseMutex();
        }

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");
                await button.ScaleTo(0.95, 100, Easing.SinInOut);

                quickCalcValue = -int.Parse(button.Text);
                isRunQuickCalc = false;

                if (Device.RuntimePlatform == Device.UWP)
                {
                    QuickCalc();
                }
                else
                {
                    quickCalcTimer.Start();
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
                    quickCalcTimer.Stop();

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
