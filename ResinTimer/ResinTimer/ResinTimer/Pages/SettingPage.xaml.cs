using ResinTimer.Dialogs;
using ResinTimer.Pages.UtilPages;
using ResinTimer.Resources;
using ResinTimer.Services;

using Rg.Plugins.Popup.Services;

using System;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        private string[] StartScreenList => new string[]
        {
            AppResources.MasterDetail_MasterList_TimerMain,
            AppResources.MasterDetail_MasterList_Resin,
            AppResources.MasterDetail_MasterList_RealmCurrency,
            AppResources.MasterDetail_MasterList_RealmFriendship,
            AppResources.MasterDetail_MasterList_Expedition,
            AppResources.MasterDetail_MasterList_GatheringItem,
            AppResources.MasterDetail_MasterList_Gadget,
            AppResources.MasterDetail_MasterList_Furnishing,
            AppResources.MasterDetail_MasterList_Gardening,
            AppResources.MasterDetail_MasterList_Talent,
            AppResources.MasterDetail_MasterList_WeaponAscension
        };
        private string[] AppLangList => new string[]
        {
            AppResources.SettingPage_Section_App_AppLang_Dialog_Default,
            "English",
            "한국어"
        };

        public SettingPage()
        {
            InitializeComponent();

            InitSpecificPlatformSetting();
            LoadSettingValue();
        }

        private void InitSpecificPlatformSetting()
        {
            if (true)//Device.RuntimePlatform is not Device.UWP)
            {
                AppSettingSection.Remove(BackgroundTrayServiceSetting);
            }

            if (Device.RuntimePlatform is not Device.Android)
            {
                AppSettingSection.Remove(ReturnStartPageSetting);
            }
        }

        private void LoadSettingValue()
        {
            // App Section
            Notification.On = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
            StartDetailScreenNow.Text = GetStartScreenString(
                Preferences.Get(SettingConstants.APP_START_DETAILSCREEN, 0));
            Use24HTimeFormat.On = Preferences.Get(SettingConstants.APP_USE_24H_TIMEFORMAT, false);
            AppLangNow.Text = AppLangList[Preferences.Get(SettingConstants.APP_LANG, (int)AppEnv.AppLang.System)];
            AppInGameServerNow.Text = AppEnv.ServerList[Preferences.Get(SettingConstants.APP_INGAMESERVER, 0)];

            if (Device.RuntimePlatform is Device.UWP)
            {
                BackgroundTrayServiceSetting.On = Preferences.Get(SettingConstants.APP_BACKGROUNDTRAYSERVICE_ENABLED, false);
            }

            if (Device.RuntimePlatform is Device.Android)
            {
                ReturnStartPageSetting.On = Preferences.Get(SettingConstants.APP_RETURNSTARTPAGE_ENABLED, true);
            }

            // Timer Common Section
            ShowOverflow.On = Preferences.Get(SettingConstants.SHOW_OVERFLOW, false);
            QuickCalcVibration.IsEnabled = Device.RuntimePlatform is not Device.UWP;
            QuickCalcVibration.On = Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            bool isAccountSyncEnabled = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_ENABLED, false);

            TimerSyncSetting.IsEnabled = isAccountSyncEnabled;
            TimerSyncSettingArrowLabel.Text = isAccountSyncEnabled ? "▶" : "X";
        }

        private string GetStartScreenString(int value)
        {
            return value switch
            {
                1 => AppResources.MasterDetail_MasterList_Resin,
                2 => AppResources.MasterDetail_MasterList_RealmCurrency,
                3 => AppResources.MasterDetail_MasterList_RealmFriendship,
                4 => AppResources.MasterDetail_MasterList_Expedition,
                5 => AppResources.MasterDetail_MasterList_GatheringItem,
                6 => AppResources.MasterDetail_MasterList_Gadget,
                7 => AppResources.MasterDetail_MasterList_Furnishing,
                8 => AppResources.MasterDetail_MasterList_Gardening,
                9 => AppResources.MasterDetail_MasterList_Talent,
                10 => AppResources.MasterDetail_MasterList_WeaponAscension,
                _ => AppResources.MasterDetail_MasterList_TimerMain
            };
        }

        private void QuickCalcVibrationOnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.QUICKCALC_VIBRATION, e.Value);
        }

        private async void AccountSyncTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountSyncPages.AccountSyncStatusPage(), true);
        }

        private async void TimerSyncSettingTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountSyncPages.TimerSyncConfigPage(), true);
        }

        private async void NotificationOnChanged(object sender, ToggledEventArgs e)
        {
            IBootService bootService = DependencyService.Get<IBootService>();
            bool isToggled = e.Value;

            Preferences.Set(SettingConstants.NOTI_ENABLED, isToggled);

            if (isToggled)
            {
                await NotiScheduleService.VerifyNotificationAvailable();

                if (Device.RuntimePlatform is Device.UWP)
                {
                    if (!await bootService.Register())
                    {
                        string title = AppResources.Bootstrap_ChangeEnableFail_Title;
                        string message = AppResources.Bootstrap_ChangeEnableFail_Message;
                        string ok = AppResources.Dialog_Ok;

                        await DisplayAlert(title, message, ok);
                    }
                }

                DependencyService.Get<INotiScheduleService>().ScheduleAll();
            }
            else
            {
                DependencyService.Get<INotiScheduleService>().CancelAll();

                if (Device.RuntimePlatform is Device.UWP)
                {
                    await bootService.Unregister();
                }
            }
        }

        private void Use24HTimeFormatOnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.APP_USE_24H_TIMEFORMAT, e.Value);
        }

        //private void SelectVisibleMenuTapped(object sender, EventArgs e)
        //{
        //    // Add open BaseDialog - CheckBoxPreferenceView code
        //}

        private async void StartDetailScreenTapped(object sender, EventArgs e)
        {
            BaseDialog dialog = new(AppResources.SettingPage_Section_App_Start_DetailScreen_Dialog_Title,
                new RadioPreferenceView(StartScreenList, SettingConstants.APP_START_DETAILSCREEN));

            dialog.OnClose += delegate 
            { 
                StartDetailScreenNow.Text = GetStartScreenString(
                    Preferences.Get(SettingConstants.APP_START_DETAILSCREEN, 0)); 
            };

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private void ReturnStartPageSettingOnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.APP_RETURNSTARTPAGE_ENABLED, e.Value);
        }

        private async void AppLangTapped(object sender, EventArgs e)
        {
            BaseDialog dialog = new(AppResources.SettingPage_Section_App_AppLang_Dialog_Title,
                new RadioPreferenceView(AppLangList, SettingConstants.APP_LANG));

            dialog.OnClose += delegate 
            { 
                AppLangNow.Text = AppLangList[Preferences.Get(SettingConstants.APP_LANG, (int)AppEnv.AppLang.System)];
            };

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void AppInGameServerTapped(object sender, EventArgs e)
        {
            BaseDialog dialog = new(AppResources.SettingPage_Section_App_InGameServer_Dialog_Title,
                new RadioPreferenceView(AppEnv.ServerList, SettingConstants.APP_INGAMESERVER));

            dialog.OnClose += delegate 
            { 
                AppInGameServerNow.Text = AppEnv.ServerList[Preferences.Get(SettingConstants.APP_INGAMESERVER, 0)];
            };

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void CheckmiHoYoAPIStatusTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CheckmiHoYoAPIStatusPage());
        }

        private void ShowOverflowOnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.SHOW_OVERFLOW, e.Value);
        }

        private void BackgroundTrayServiceSettingOnChanged(object sender, ToggledEventArgs e)
        {
            if (sender is SwitchCell cell)
            {
                Preferences.Set(SettingConstants.APP_BACKGROUNDTRAYSERVICE_ENABLED, e.Value);
            }
        }
    }
}