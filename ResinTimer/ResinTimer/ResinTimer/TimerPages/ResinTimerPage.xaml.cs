using GenshinInfo.Constants;
using GenshinInfo.Managers;

using ResinTimer.Dialogs;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.NotiSettingPages;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppEnv = ResinTimer.AppEnvironment;
using Timer = System.Timers.Timer;
using TTimer = System.Threading.Timer;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResinTimerPage : ContentPage
    {
        public ICommand UrlOpenTabCommand => Utils.UrlOpenCommand;

        private Timer buttonPressTimer;
        private TTimer calcTimer;

        private int quickCalcValue;
        private int quickOTCalcValue;
        private bool isRunQuickCalc = false;
        private bool isQuickOTCalc = false;

        public ResinTimerPage()
        {
            InitializeComponent();

            BindingContext = this;

            ResinEnvironment.oneCountTime = new ResinTime(0);
            ResinEnvironment.totalCountTime = new ResinTime(0);

            ResinEnvironment.LoadValues();

            if (!(Device.RuntimePlatform == Device.UWP))
            {
                buttonPressTimer = new(500)
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

            if (ResinEnvironment.IsSyncEnabled)
            {
                _ = SyncData();
            }
        }

        private void SetToolbar()
        {
            NotiToolbarItem.IsEnabled = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetToolbar();
            SetLayoutAppearance();

            //calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.5));
            calcTimer = new(CalcTimeResin, new AutoResetEvent(false), TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.5));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            calcTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            calcTimer.Dispose();

            ResinEnvironment.SaveValue();
        }

        private void SetLayoutAppearance()
        {
            bool isSyncEnabled = ResinEnvironment.IsSyncEnabled;

            CautionResinOnlyLabel.IsVisible = !isSyncEnabled &&
                (ResinEnvironment.applyType == ResinEnvironment.ApplyType.Resin);
            
            ManualControlLayout.IsVisible = !isSyncEnabled;
            SyncControlLayout.IsVisible = isSyncEnabled;

            if (isSyncEnabled)
            {
                ToolbarItems.Remove(EditToolbarItem);
            }
        }

        private async void ToolbarItemClicked(object sender, EventArgs e)
        {
            ToolbarItem item = sender as ToolbarItem;

            switch (item.Text)
            {
                case "Edit":
                    await Navigation.PushAsync(new EditResinPage(), true);
                    break;
                case "Noti Setting":
                    await Navigation.PushAsync(new ResinNotiSettingPage(), true);
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
            catch (Exception ex)
            {
#if DEBUG
                DependencyService.Get<IToast>().Show(ex.ToString());
#endif
            }
        }

        private void RefreshInfo()
        {
            try
            {
                TotalTimeHour.Text = $"{ResinEnvironment.totalCountTime.Hour:D2}";
                TotalTimeMinute.Text = $"{ResinEnvironment.totalCountTime.Min:D2}";

                LastInputDateTimeLabel.Text = Utils.GetTimeString(DateTime.Parse(ResinEnvironment.lastInputTime, AppEnv.dtCulture));
                EndDateTimeLabel.Text = Utils.GetTimeString(ResinEnvironment.endTime);

                ResinCount.Text = ResinEnvironment.resin.ToString();
                OneCountTimer.Text = ResinEnvironment.oneCountTime.TimeMinSec;

                RangeValue.Value = ResinEnvironment.resin;
                PointerValue.Value = ResinEnvironment.resin;

                ResinRemainTimeRange.EndValue = 160 - ResinEnvironment.oneCountTime.TotalSec * ((double)ResinEnvironment.MAX_RESIN / ResinTime.ONE_RESTORE_INTERVAL);

                int overflowValue = ResinEnvironment.CalcResinOverflow();

                ResinOverflowLabel.Text = (Preferences.Get(SettingConstants.SHOW_OVERFLOW, false) && (overflowValue > 0)) ? $"{AppResources.Overflow_Text} : {overflowValue}" : "";
            }
            catch (Exception) { }
            finally
            {
                System.Diagnostics.Debug.WriteLine("Resin info refresh");
            }
        }

        private void QuickCalc()
        {
            DateTime now = DateTime.Now;

            ResinEnvironment.lastInputTime = now.ToString(AppEnv.dtCulture);

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

            UpdateSaveData();
        }

        private async Task SyncData()
        {
            SyncStatusTipLabel.IsVisible = false;
            ManualSyncButton.IsEnabled = false;
            SyncStatusLabel.TextColor = Color.Default;
            SyncStatusLabel.Text = AppResources.Sync_Working;

            await Task.Delay(100);

            if (await ResinEnvironment.SyncServerData())
            {
                UpdateSaveData();

                SyncStatusLabel.TextColor = Color.Green;
                SyncStatusLabel.Text = AppResources.Sync_Success;
            }
            else
            {
                SyncStatusLabel.TextColor = Color.OrangeRed;
                SyncStatusLabel.Text = AppResources.Sync_Fail;
                SyncStatusTipLabel.IsVisible = true;
            }

            ManualSyncButton.IsEnabled = true;
        }

        private void UpdateSaveData()
        {
            ResinEnvironment.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                ResinNotiManager notiManager = new();

                notiManager.UpdateNotisTime();
            }
        }

        private async void ButtonPressed(object sender, EventArgs e)
        {
            Button button = sender as Button;

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
            Button button = sender as Button;

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

        private async void QEButtonPressed(object sender, EventArgs e)
        {
            Button button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");

                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void QEButtonReleased(object sender, EventArgs e)
        {
            Button button = sender as Button;

            try
            {
                button.BackgroundColor = Color.Transparent;

                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void QEButton_Clicked(object sender, EventArgs e)
        {
            BaseDialog dialog = new(AppResources.ResinSimpleEditDialog_Title, new ResinSimpleEdit());

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void ManualSyncButton_Clicked(object sender, EventArgs e)
        {
            await SyncData();
        }
    }
}