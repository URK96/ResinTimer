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
using RealmEnv = ResinTimer.RealmEnvironment;
using RFEnv = ResinTimer.RealmFriendshipEnvironment;
using Timer = System.Timers.Timer;
using TTimer = System.Threading.Timer;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealmFriendshipTimerPage : ContentPage
    {
        private Timer _buttonPressTimer;
        private TTimer _calcTimer;

        private bool _isRunReset;

        public RealmFriendshipTimerPage()
        {
            InitializeComponent();

            RFEnv.LoadValues();

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

                    ResetRF();
                };
            }

            _calcTimer = new(CalcNowRF, new AutoResetEvent(false), TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
        }

        private void SetToolbar()
        {
            NotiToolbarItem.IsEnabled = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));

            SetRFStatusLabel();
            SetToolbar();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            RFEnv.SaveValue();

            _calcTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void SetRFStatusLabel()
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
                $"{AppResources.RealmCurrency_RealmRank} : {realmRankString} (+{RFEnv.RFRate} / 1H)";
            TrustLevelLabel.Text = 
                $"{AppResources.RealmCurrency_TrustRank} : {RealmEnv.TrustRank} (Max {RFEnv.MaxRF})";
        }

        private async void ToolbarItemClicked(object sender, EventArgs e)
        {
            var item = sender as ToolbarItem;

            switch (item.Text)
            {
                case "Edit":
                    await Navigation.PushAsync(new EditRealmSetting(RealmEnv.RealmType.Friendship), true);
                    break;
                case "Noti Setting":
                    await Navigation.PushAsync(new RealmFriendshipNotiSettingPage(), true);
                    break;
                default:
                    break;
            }
        }

        private void CalcNowRF(object statusInfo)
        {
            try
            {
                var now = DateTime.Now;

                RFEnv.TotalCountTime = (RFEnv.EndTime > now) ? (RFEnv.EndTime - now) : TimeSpan.FromSeconds(0);

                RFEnv.CalcRF();

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
                TotalTimeHour.Text = $"{(int)RFEnv.TotalCountTime.TotalHours}";
                TotalTimeMinute.Text = $"{RFEnv.TotalCountTime.Minutes:D2}";

                LastInputDateTimeLabel.Text = Utils.GetTimeString(DateTime.Parse(RFEnv.LastInputTime, AppEnv.DTCulture));
                EndDateTimeLabel.Text = Utils.GetTimeString(RFEnv.EndTime);

                RFSfScale.EndValue = 100;
                RFSfScale.Interval = 20;

                int percentage = RFEnv.Percentage;

                RFPercentage.Text = $"{percentage} %";
                RangeValue.Value = PointerValue.Value = percentage;

                RFCountLabel.Text = $"{AppResources.RealmCurrency_NowCurrency} : {RFEnv.Bounty}";

            }
            catch (Exception ex)
            {
#if DEBUG
                DependencyService.Get<IToast>().Show(ex.ToString());
#endif
            }
        }

        private void ResetRF()
        {
            DateTime now = DateTime.Now;
            int count = RFEnv.MaxRF / RFEnv.RFRate;

            count += (RFEnv.MaxRF % RFEnv.RFRate) == 0 ? 0 : 1;

            RFEnv.LastInputTime = now.ToString(AppEnv.DTCulture);

#if TEST
            RFEnv.EndTime = now.AddMinutes(count);
#else
            RFEnv.EndTime = now.AddHours(count);
#endif

            RFEnv.Bounty = 0;
            RFEnv.AddCount = 0;

            RFEnv.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                new RealmFriendshipNotiManager().UpdateNotisTime();
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
                    ResetRF();
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

            var dialog = new BaseDialog(AppResources.BountySimpleEditDialog_Title, new RealmFriendshipSimpleEdit());

            dialog.OnClose += delegate { _calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)); };

            await PopupNavigation.Instance.PushAsync(dialog);
        }
    }
}