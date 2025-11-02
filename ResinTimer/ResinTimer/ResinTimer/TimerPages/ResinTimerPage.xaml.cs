using ResinTimer.Dialogs;
using ResinTimer.Helper;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.NotiSettingPages;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls.Xaml;

using AppEnv = ResinTimer.AppEnvironment;
using REnv = ResinTimer.ResinEnvironment;
using Timer = System.Timers.Timer;
using TTimer = System.Threading.Timer;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResinTimerPage : ContentPage
    {
        public ICommand UrlOpenTabCommand => Utils.UrlOpenCommand;

        private Timer _buttonPressTimer;
        private TTimer _calcTimer;

        private int _quickCalcValue;
        private int _quickOTCalcValue;
        private bool _isRunQuickCalc = false;
        private bool _isQuickOTCalc = false;

        public ResinTimerPage()
        {
            InitializeComponent();

            BindingContext = this;

            REnv.OneCountTime = new();
            REnv.TotalCountTime = new();

            REnv.LoadValues();

            // TODO Xamarin.Forms.Device.RuntimePlatform(은)는 더 이상 지원되지 않습니다. 대신 Microsoft.Maui.Devices.DeviceInfo.Platform을(를) 사용하세요. 자세한 내용은 https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes(을)를 참조하세요.
            if (Device.RuntimePlatform is not Device.UWP)
            {
                _buttonPressTimer = new(500)
                {
                    AutoReset = false
                };
                _buttonPressTimer.Elapsed += delegate
                {
                    _isRunQuickCalc = true;

                    if (Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true))
                    {
                        Vibration.Vibrate(100);
                    }

                    QuickCalc();
                };
            }

            if (REnv.IsSyncEnabled)
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

            _calcTimer = new(CalcTimeResin, new AutoResetEvent(false), 
                             TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.5));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _calcTimer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _calcTimer?.Dispose();

            REnv.SaveValue();
        }

        private void SetLayoutAppearance()
        {
            bool isSyncEnabled = REnv.IsSyncEnabled;

            CautionResinOnlyLabel.IsVisible = !isSyncEnabled &&
                (REnv.ManualApplyType is REnv.ApplyType.Resin);
            ManualControlLayout.IsVisible = !isSyncEnabled;
            SyncControlLayout.IsVisible = isSyncEnabled;

            if (isSyncEnabled)
            {
                ToolbarItems.Remove(EditToolbarItem);
            }
        }

        private async void ToolbarItemClicked(object sender, EventArgs e)
        {
            var item = sender as ToolbarItem;

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
                if (REnv.EndTime >= DateTime.Now)
                {
                    REnv.CalcResinTime();
                    REnv.CalcResin();
                }
                else
                {
                    REnv.TotalCountTime = new();
                    REnv.OneCountTime = new();
                    REnv.Resin = REnv.MaxResin;
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
                TotalTimeHour.Text = $"{REnv.TotalCountTime.Hours:D2}";
                TotalTimeMinute.Text = $"{REnv.TotalCountTime.Minutes:D2}";

                LastInputDateTimeLabel.Text = Utils.GetTimeString(
                    DateTime.Parse(REnv.LastInputTime, AppEnv.DTCulture));
                EndDateTimeLabel.Text = Utils.GetTimeString(REnv.EndTime);

                ResinCount.Text = REnv.Resin.ToString();
                OneCountTimer.Text = $"{REnv.OneCountTime.Minutes} : {REnv.OneCountTime.Seconds:D2}";

                RangeValue.Value = REnv.Resin;
                PointerValue.Value = REnv.Resin;

                ResinRemainTimeRange.EndValue = REnv.MaxResin - REnv.OneCountTime.TotalSeconds * 
                    ((double)REnv.MaxResin / REnv.OneRestoreInterval);

                int overflowValue = REnv.CalcResinOverflow();

                ResinOverflowLabel.Text = (Preferences.Get(
                    SettingConstants.SHOW_OVERFLOW, false) && (overflowValue > 0)) ? 
                    $"{AppResources.Overflow_Text} : {overflowValue}" : "";
            }
            catch (Exception) { }
            finally
            {
                //System.Diagnostics.Debug.WriteLine("Resin info refresh");
            }
        }

        private void QuickCalc()
        {
            DateTime now = DateTime.Now;

            REnv.LastInputTime = now.ToString(AppEnv.DTCulture);

            if (REnv.EndTime < now)
            {
                REnv.EndTime = now;
            }

            if (_isQuickOTCalc)
            {
                REnv.EndTime = REnv.EndTime.AddSeconds(
                    REnv.OneRestoreInterval * (REnv.Resin / _quickOTCalcValue) * _quickOTCalcValue);

                _isQuickOTCalc = false;
            }
            else
            {
                REnv.EndTime = ((REnv.Resin - _quickCalcValue) < 0) ?
                    now.AddSeconds(REnv.OneRestoreInterval * REnv.MaxResin) :
                    REnv.EndTime.AddSeconds(REnv.OneRestoreInterval * _quickCalcValue);
            }

            REnv.UpdateSaveData();
        }

        private async Task SyncData()
        {
            SyncStatusTipLabel.IsVisible = false;
            ManualSyncButton.IsEnabled = false;
            ManualSyncButton.BorderColor = null;

            await Task.Delay(100);

            if (await SyncHelper.Update(SyncHelper.SyncTarget.Resin))
            {
                REnv.UpdateSaveData();

                ManualSyncButton.BorderColor = Colors.Green;
            }
            else
            {
                ManualSyncButton.BorderColor = Colors.OrangeRed;
                SyncStatusTipLabel.IsVisible = true;
            }

            ManualSyncButton.IsEnabled = true;
        }

        //private void UpdateSaveData()
        //{
        //    REnv.SaveValue();

        //    if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
        //    {
        //        ResinNotiManager notiManager = new();

        //        notiManager.UpdateNotisTime();
        //        notiManager.UpdateScheduledNoti<ResinNoti>();
        //    }
        //}

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromArgb("#500682F6");

                await button.ScaleTo(0.95, 100, Easing.SinInOut);

                if (!int.TryParse(button.Text, out _quickCalcValue))
                {
                    _quickOTCalcValue = int.Parse(button.Text.Substring(2));

                    _isQuickOTCalc = true;
                }
                else
                {
                    _quickCalcValue = -_quickCalcValue;
                }
                
                _isRunQuickCalc = false;

                // TODO Xamarin.Forms.Device.RuntimePlatform(은)는 더 이상 지원되지 않습니다. 대신 Microsoft.Maui.Devices.DeviceInfo.Platform을(를) 사용하세요. 자세한 내용은 https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes(을)를 참조하세요.
                if (Device.RuntimePlatform is Device.UWP)
                {
                    QuickCalc();
                }
                else
                {
                    _buttonPressTimer.Start();
                }
            }
            catch { }
        }

        private async void ButtonReleased(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Colors.Transparent;

                await button.ScaleTo(1.0, 100, Easing.SinInOut);

                // TODO Xamarin.Forms.Device.RuntimePlatform(은)는 더 이상 지원되지 않습니다. 대신 Microsoft.Maui.Devices.DeviceInfo.Platform을(를) 사용하세요. 자세한 내용은 https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes(을)를 참조하세요.
                if (Device.RuntimePlatform is not Device.UWP)
                {
                    _buttonPressTimer.Stop();

                    if (!_isRunQuickCalc)
                    {
                        DependencyService.Get<IToast>().Show(AppResources.MainPage_QuickCalcButton_Toast);
                    }
                }
            }
            catch { }
        }

        private async void QEButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromArgb("#500682F6");

                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void QEButtonReleased(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Colors.Transparent;

                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void QEButtonClicked(object sender, EventArgs e)
        {
            BaseDialog dialog = new(AppResources.ResinSimpleEditDialog_Title, new ResinSimpleEdit());

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void ManualSyncButtonClicked(object sender, EventArgs e)
        {
            await SyncData();
        }
    }
}