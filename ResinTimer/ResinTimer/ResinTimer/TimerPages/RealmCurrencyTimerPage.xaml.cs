using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Timer = System.Timers.Timer;
using TTimer = System.Threading.Timer;
using RCEnv = ResinTimer.RealmCurrencyEnvironment;
using AppEnv = ResinTimer.AppEnvironment;
using Xamarin.Essentials;
using System.Threading;
using ResinTimer.Resources;
using ResinTimer.Dialogs;
using Rg.Plugins.Popup.Services;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealmCurrencyTimerPage : ContentPage
    {
        private Timer buttonPressTimer;
        private TTimer calcTimer;

        private bool isRunReset;

        public RealmCurrencyTimerPage()
        {
            InitializeComponent();

            RCEnv.LoadValues();

            if (Device.RuntimePlatform != Device.UWP)
            {
                buttonPressTimer = new Timer(500)
                {
                    AutoReset = false
                };
                buttonPressTimer.Elapsed += delegate
                {
                    isRunReset = true;

                    if (Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true))
                    {
                        Vibration.Vibrate(100);
                    }

                    ResetRC();
                };
            }

            calcTimer = new TTimer(CalcNowRC, new AutoResetEvent(false), TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
        }

        private void SetToolbar()
        {
            NotiToolbarItem.IsEnabled = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));

            SetRCStatusLabel();

            SetToolbar();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            RCEnv.SaveValue();

            calcTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void SetRCStatusLabel()
        {
            var realmRankString = RCEnv.realmRank switch
            {
                RCEnv.RealmRank.HumbleAbode => AppResources.Realm_Rank_2,
                RCEnv.RealmRank.Cozy => AppResources.Realm_Rank_3,
                RCEnv.RealmRank.QueenSize => AppResources.Realm_Rank_4,
                RCEnv.RealmRank.Elegant => AppResources.Realm_Rank_5,
                RCEnv.RealmRank.Exquisite => AppResources.Realm_Rank_6,
                RCEnv.RealmRank.Extraordinary => AppResources.Realm_Rank_7,
                RCEnv.RealmRank.Stately => AppResources.Realm_Rank_8,
                RCEnv.RealmRank.Luxury => AppResources.Realm_Rank_9,
                RCEnv.RealmRank.KingFit => AppResources.Realm_Rank_10,
                _ => AppResources.Realm_Rank_1
            };

            RealmRankLabel.Text = $"{AppResources.RealmCurrency_RealmRank} : {realmRankString} (+{RCEnv.RCRate} / Hour)";
            TrustLevelLabel.Text = $"{AppResources.RealmCurrency_TrustRank} : {RCEnv.trustRank} (Max {RCEnv.MaxRC})";
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var item = sender as ToolbarItem;

            switch (item.Text)
            {
                case "Edit":
                    await Navigation.PushAsync(new EditRealmCurrencySetting(), true);
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

                RCEnv.totalCountTime = (RCEnv.endTime > now) ? (RCEnv.endTime - now) : TimeSpan.FromSeconds(0);

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
                TotalTimeHour.Text = $"{(int)RCEnv.totalCountTime.TotalHours}";
                TotalTimeMinute.Text = $"{RCEnv.totalCountTime.Minutes:D2}";

                LastInputDateTimeLabel.Text = AppEnv.GetTimeString(DateTime.Parse(RCEnv.lastInputTime, AppEnv.dtCulture));
                EndDateTimeLabel.Text = AppEnv.GetTimeString(RCEnv.endTime);

                RCSfScale.EndValue = 100;
                RCSfScale.Interval = 20;

                int percentage = Convert.ToInt32((double)RCEnv.currency / RCEnv.MaxRC * 100);

                RCPercentage.Text = $"{percentage} %";
                RangeValue.Value = PointerValue.Value = percentage;

                RCCountLabel.Text = $"{AppResources.RealmCurrency_NowCurrency} : {RCEnv.currency}";

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
            var now = DateTime.Now;
            int count = RCEnv.MaxRC / RCEnv.RCRate;

            count += (RCEnv.MaxRC % RCEnv.RCRate) == 0 ? 0 : 1;

            RCEnv.lastInputTime = now.ToString(AppEnv.dtCulture);

#if TEST
            RCEnv.endTime = now.AddMinutes(count);
#else
            RCEnv.endTime = now.AddHours(count);
#endif

            RCEnv.currency = 0;
            RCEnv.addCount = 0;

            RCEnv.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                var notiManager = new RealmCurrencyNotiManager();
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

                isRunReset = false;

                if (Device.RuntimePlatform == Device.UWP)
                {
                    ResetRC();
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

                if (Device.RuntimePlatform != Device.UWP)
                {
                    buttonPressTimer.Stop();

                    if (!isRunReset)
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

        private async void QEButton_Clicked(object sender, EventArgs e)
        {
            calcTimer.Change(Timeout.Infinite, Timeout.Infinite);

            var dialog = new BaseDialog(AppResources.CurrencySimpleEditDialog_Title, new RealmCurrencySimpleEdit());

            dialog.OnClose += delegate { calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)); };

            await PopupNavigation.Instance.PushAsync(dialog);
        }
    }
}