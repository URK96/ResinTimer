using ResinTimer.Dialogs;
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
        private Timer buttonPressTimer;
        private TTimer calcTimer;

        private bool isRunReset;

        public RealmFriendshipTimerPage()
        {
            InitializeComponent();

            RFEnv.LoadValues();

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

                    ResetRF();
                };
            }

            calcTimer = new TTimer(CalcNowRF, new AutoResetEvent(false), TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
        }

        private void SetToolbar()
        {
            NotiToolbarItem.IsEnabled = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));

            SetRFStatusLabel();

            SetToolbar();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            RFEnv.SaveValue();

            calcTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void SetRFStatusLabel()
        {
            var realmRankString = RealmEnv.realmRank switch
            {
                RealmEnv.RealmRank.HumbleAbode => AppResources.Realm_Rank_2,
                RealmEnv.RealmRank.Cozy => AppResources.Realm_Rank_3,
                RealmEnv.RealmRank.QueenSize => AppResources.Realm_Rank_4,
                RealmEnv.RealmRank.Elegant => AppResources.Realm_Rank_5,
                RealmEnv.RealmRank.Exquisite => AppResources.Realm_Rank_6,
                RealmEnv.RealmRank.Extraordinary => AppResources.Realm_Rank_7,
                RealmEnv.RealmRank.Stately => AppResources.Realm_Rank_8,
                RealmEnv.RealmRank.Luxury => AppResources.Realm_Rank_9,
                RealmEnv.RealmRank.KingFit => AppResources.Realm_Rank_10,
                _ => AppResources.Realm_Rank_1
            };

            RealmRankLabel.Text = $"{AppResources.RealmCurrency_RealmRank} : {realmRankString} (+{RFEnv.RFRate} / Hour)";
            TrustLevelLabel.Text = $"{AppResources.RealmCurrency_TrustRank} : {RealmEnv.trustRank} (Max {RFEnv.MaxRF})";
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var item = sender as ToolbarItem;

            switch (item.Text)
            {
                case "Edit":
                    await Navigation.PushAsync(new EditRealmSetting(), true);
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

                RFEnv.totalCountTime = (RFEnv.endTime > now) ? (RFEnv.endTime - now) : TimeSpan.FromSeconds(0);

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
                TotalTimeHour.Text = $"{(int)RFEnv.totalCountTime.TotalHours}";
                TotalTimeMinute.Text = $"{RFEnv.totalCountTime.Minutes:D2}";

                LastInputDateTimeLabel.Text = AppEnv.GetTimeString(DateTime.Parse(RFEnv.lastInputTime, AppEnv.dtCulture));
                EndDateTimeLabel.Text = AppEnv.GetTimeString(RFEnv.endTime);

                RFSfScale.EndValue = 100;
                RFSfScale.Interval = 20;

                int percentage = Convert.ToInt32((double)RFEnv.bounty / RFEnv.MaxRF * 100);

                RFPercentage.Text = $"{percentage} %";
                RangeValue.Value = PointerValue.Value = percentage;

                RFCountLabel.Text = $"{AppResources.RealmCurrency_NowCurrency} : {RFEnv.bounty}";

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
            var now = DateTime.Now;
            int count = RFEnv.MaxRF / RFEnv.RFRate;

            count += (RFEnv.MaxRF % RFEnv.RFRate) == 0 ? 0 : 1;

            RFEnv.lastInputTime = now.ToString(AppEnv.dtCulture);

#if TEST
            RFEnv.endTime = now.AddMinutes(count);
#else
            RFEnv.endTime = now.AddHours(count);
#endif

            RFEnv.bounty = 0;
            RFEnv.addCount = 0;

            RFEnv.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                var notiManager = new RealmFriendshipNotiManager();
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
                    ResetRF();
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

            var dialog = new BaseDialog(AppResources.BountySimpleEditDialog_Title, new RealmBountySimpleEdit());

            dialog.OnClose += delegate { calcTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)); };

            await PopupNavigation.Instance.PushAsync(dialog);
        }
    }
}