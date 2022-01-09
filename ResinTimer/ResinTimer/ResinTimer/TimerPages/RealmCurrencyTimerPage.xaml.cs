using ResinTimer.Dialogs;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.NotiSettingPages;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Threading;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppEnv = ResinTimer.AppEnvironment;
using RCEnv = ResinTimer.RealmCurrencyEnvironment;
using RealmEnv = ResinTimer.RealmEnvironment;
using Timer = System.Timers.Timer;
using TTimer = System.Threading.Timer;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealmCurrencyTimerPage : ContentPage
    {
        private Timer _buttonPressTimer;
        private TTimer _calcTimer;

        private bool _isRunReset;

        public RealmCurrencyTimerPage()
        {
            InitializeComponent();

            RCEnv.LoadValues();

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

            _calcTimer = new(CalcNowRC, new AutoResetEvent(false), TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
        }

        private void SetToolbar()
        {
            NotiToolbarItem.IsEnabled = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));

            SetRCStatusLabel();
            SetToolbar();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            RCEnv.SaveValue();

            _calcTimer.Change(Timeout.Infinite, Timeout.Infinite);
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

            RealmRankLabel.Text = $"{AppResources.RealmCurrency_RealmRank} : {realmRankString} (+{RCEnv.RCRate} / 1H)";
            TrustLevelLabel.Text = $"{AppResources.RealmCurrency_TrustRank} : {RealmEnv.TrustRank} (Max {RCEnv.MaxRC})";
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
                var now = DateTime.Now;

                RCEnv.TotalCountTime = (RCEnv.EndTime > now) ? (RCEnv.EndTime - now) : TimeSpan.FromSeconds(0);

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

                LastInputDateTimeLabel.Text = Utils.GetTimeString(DateTime.Parse(RCEnv.LastInputTime, AppEnv.DTCulture));
                EndDateTimeLabel.Text = Utils.GetTimeString(RCEnv.EndTime);

                RCSfScale.EndValue = 100;
                RCSfScale.Interval = 20;

                int percentage = RCEnv.Percentage;

                RCPercentage.Text = $"{percentage} %";
                RangeValue.Value = PointerValue.Value = percentage;

                RCCountLabel.Text = $"{AppResources.RealmCurrency_NowCurrency} : {RCEnv.Currency}";

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

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");

                await button.ScaleTo(0.95, 100, Easing.SinInOut);

                _isRunReset = false;

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
                button.BackgroundColor = Color.Transparent;

                await button.ScaleTo(1.0, 100, Easing.SinInOut);

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
                button.BackgroundColor = Color.FromHex("#500682F6");

                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void QEButtonReleased(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.Transparent;

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
    }
}