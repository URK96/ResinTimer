using System;
using System.Threading;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace ResinTimer
{
    public partial class MainPage : ContentPage
    {
        private Mutex mutex;

        private Thread calcThread;

        public MainPage()
        {
            InitializeComponent();

            var animation = new Animation();
            var fadeOutAnimation = new Animation(v => TimeSeperator.Opacity = v, 1, 0);
            var fadeInAnimation = new Animation(v => TimeSeperator.Opacity = v, 0, 1);

            animation.Add(0, 0.5, fadeOutAnimation);
            animation.Add(0.5, 1, fadeInAnimation);
            animation.Commit(this, "TimeSeperatorAnimation", 16, 2000, Easing.BounceIn, repeat: () => true);

            ResinEnvironment.oneCountTime = new ResinTime(0);
            ResinEnvironment.totalCountTime = new ResinTime(0);

            ResinEnvironment.LoadValues();

            mutex = new Mutex(false);

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
    }
}
