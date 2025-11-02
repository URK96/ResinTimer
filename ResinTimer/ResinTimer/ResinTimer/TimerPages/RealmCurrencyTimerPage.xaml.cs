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
using RCEnv = ResinTimer.RealmCurrencyEnvironment;
using RealmEnv = ResinTimer.RealmEnvironment;
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
    public partial class RealmCurrencyTimerPage : ContentPage
    {
        public ICommand UrlOpenTabCommand => Utils.UrlOpenCommand;

        private Timer _buttonPressTimer;
        private TTimer _calcTimer;

        private bool _isRunReset;

        public RealmCurrencyTimerPage()
        {
            InitializeComponent();

            BindingContext = this;

            RCEnv.LoadValues();

            // TODO Xamarin.Forms.Device.RuntimePlatform(은)는 더 이상 지원되지 않습니다. 대신 Microsoft.Maui.Devices.DeviceInfo.Platform을(를) 사용하세요. 자세한 내용은 https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes(을)를 참조하세요.
            if (Device.RuntimePlatform is not Device.UWP)
            {
                _buttonPressTimer = new(500)
                {
                    AutoReset = false
                };
                _buttonPressTimer.Elapsed += delegate
                {
                    _isRunReset = true;

                    if (Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true))
                    {
                        Vibration.Vibrate(100);
                    }

                    ResetRC();
                };
            }

            if (RCEnv.IsSyncEnabled)
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

            SetRCStatusLabel();
            SetToolbar();
            SetLayoutAppearance();

            _calcTimer = new(CalcNowRC, 
                new AutoResetEvent(false),
                TimeSpan.FromSeconds(0), 
                TimeSpan.FromSeconds(1));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _calcTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _calcTimer.Dispose();

            RCEnv.SaveValue();
        }

        private void SetLayoutAppearance()
        {
            bool isSyncEnabled = RCEnv.IsSyncEnabled;

            ManualControlLayout.IsVisible = !isSyncEnabled;
            SyncControlLayout.IsVisible = isSyncEnabled;

            CautionLabel.Text = isSyncEnabled ?
                AppResources.RealmCurrencyTimerPage_Caution_SyncMode :
                AppResources.RealmCurrencyTimerPage_Caution;

            if (isSyncEnabled)
            {
                ToolbarItems.Remove(EditRealmEnvironment);
            }
        }

        private void SetRCStatusLabel()
        {
            string realmRankString = RealmEnv.RealmRank switch
            {
                RealmEnv.RealmRankEnum.HumbleAbode => AppResources.Realm_Rank_2,
                RealmEnv.RealmRankEnum.Cozy => AppResources.Realm_Rank_3,
                RealmEnv.RealmRankEnum.QueenSize => AppResources.Realm_Rank_4,
                RealmEnv.RealmRankEnum.Elegant => AppResources.Realm_Rank_5,
                RealmEnv.RealmRankEnum.Exquisite => AppResources.Realm_Rank_6,
                RealmEnv.RealmRankEnum.Extraordinary => AppResources.Realm_Rank_7,
                RealmEnv.RealmRankEnum.Stately => AppResources.Realm_Rank_8,
                RealmEnv.RealmRankEnum.Luxury => AppResources.Realm_Rank_9,
                RealmEnv.RealmRankEnum.KingFit => AppResources.Realm_Rank_10,
                _ => AppResources.Realm_Rank_1
            };

            RealmRankLabel.Text =
                $"{AppResources.RealmCurrency_RealmRank} : " +
                (RCEnv.IsSyncEnabled ? "Sync Mode" : $"{realmRankString} (+{RCEnv.RCRate} / 1H)");
            TrustLevelLabel.Text = 
                $"{AppResources.RealmCurrency_TrustRank} : " +
                $"{(RCEnv.IsSyncEnabled ? "Sync Mode" : RealmEnv.TrustRank)}" +
                $" (Max {RCEnv.MaxRC})";
        }

        private async void ToolbarItemClicked(object sender, EventArgs e)
        {
            var item = sender as ToolbarItem;

            switch (item.Text)
            {
                case "Edit":
                    await Navigation.PushAsync(new EditRealmSetting(RealmEnv.RealmType.Currency), true);
                    break;
                case "Noti Setting":
                    await Navigation.PushAsync(new RealmCurrencyNotiSettingPage(), true);
                    break;
                default:
                    break;
            }
        }

        private void CalcNowRC(object statusInfo)
        {
            try
            {
                RCEnv.CalcRC();

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
                TotalTimeHour.Text = $"{(int)RCEnv.TotalCountTime.TotalHours}";
                TotalTimeMinute.Text = $"{RCEnv.TotalCountTime.Minutes:D2}";

                LastInputDateTimeLabel.Text = Utils.GetTimeString(
                    DateTime.Parse(RCEnv.LastInputTime, AppEnv.DTCulture));
                EndDateTimeLabel.Text = Utils.GetTimeString(RCEnv.EndTime);

                RCSfScale.EndValue = 100;
                RCSfScale.Interval = 20;

                int percentage = RCEnv.Percentage;

                RCPercentage.Text = $"{percentage} %";
                RangeValue.Value = PointerValue.Value = percentage;
            }
            catch (Exception ex)
            {
#if DEBUG
                DependencyService.Get<IToast>().Show(ex.ToString());
#endif
            }
        }

        private void ResetRC()
        {
            DateTime now = DateTime.Now;
            int count = RCEnv.MaxRC / RCEnv.RCRate;

            count += ((RCEnv.MaxRC % RCEnv.RCRate) == 0) ? 0 : 1;

            RCEnv.LastInputTime = now.ToString(AppEnv.DTCulture);

#if TEST
            RCEnv.EndTime = now.AddMinutes(count);
#else
            RCEnv.EndTime = now.AddHours(count);
#endif

            RCEnv.Currency = 0;
            RCEnv.AddCount = 0;

            RCEnv.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                new RealmCurrencyNotiManager().UpdateNotisTime();
            }
        }

        private async Task SyncData()
        {
            SyncStatusTipLabel.IsVisible = false;
            ManualSyncButton.IsEnabled = false;
            ManualSyncButton.BorderColor = null;

            await Task.Delay(100);

            if (await SyncHelper.Update(SyncHelper.SyncTarget.RealmCurrency))
            {
                RCEnv.UpdateSaveData();

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
        //    RCEnv.SaveValue();

        //    if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
        //    {
        //        RealmCurrencyNotiManager notiManager = new();

        //        notiManager.UpdateNotisTime();
        //        notiManager.UpdateScheduledNoti<RealmCurrencyNoti>();
        //    }
        //}

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromArgb("#500682F6");

                await button.ScaleTo(0.95, 100, Easing.SinInOut);

                _isRunReset = false;

                // TODO Xamarin.Forms.Device.RuntimePlatform(은)는 더 이상 지원되지 않습니다. 대신 Microsoft.Maui.Devices.DeviceInfo.Platform을(를) 사용하세요. 자세한 내용은 https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes(을)를 참조하세요.
                if (Device.RuntimePlatform is Device.UWP)
                {
                    ResetRC();
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

                    if (!_isRunReset)
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
            _calcTimer.Change(Timeout.Infinite, Timeout.Infinite);

            var dialog = new BaseDialog(AppResources.CurrencySimpleEditDialog_Title, new RealmCurrencySimpleEdit());

            dialog.OnClose += delegate { _calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)); };

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void ManualSyncButtonClicked(object sender, EventArgs e)
        {
            await SyncData();
        }
    }
}