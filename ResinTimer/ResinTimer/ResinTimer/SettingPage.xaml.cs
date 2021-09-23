using ResinTimer.Dialogs;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        private string[] StartScreenList => new string[]
        {
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

            LoadSettingValue();
        }

        private void LoadSettingValue()
        {
            // App Section
            Notification.On = Preferences.Get(SettingConstants.NOTI_ENABLED, false);
            StartDetailScreenNow.Text = GetStartScreenString(Preferences.Get(SettingConstants.APP_START_DETAILSCREEN, 0));
            Use24HTimeFormat.On = Preferences.Get(SettingConstants.APP_USE_24H_TIMEFORMAT, false);
            AppLangNow.Text = AppLangList[Preferences.Get(SettingConstants.APP_LANG, (int)AppEnv.AppLang.System)];
            AppInGameServerNow.Text = AppEnv.serverList[Preferences.Get(SettingConstants.APP_INGAMESERVER, 0)];

            // Timer Common Section
            ShowOverflow.On = Preferences.Get(SettingConstants.SHOW_OVERFLOW, false);
            QuickCalcVibration.IsEnabled = Device.RuntimePlatform != Device.UWP;
            QuickCalcVibration.On = Preferences.Get(SettingConstants.QUICKCALC_VIBRATION, true);
        }

        private string GetStartScreenString(int value)
        {
            return value switch
            {
                1 => AppResources.MasterDetail_MasterList_RealmCurrency,
                2 => AppResources.MasterDetail_MasterList_RealmFriendship,
                3 => AppResources.MasterDetail_MasterList_Expedition,
                4 => AppResources.MasterDetail_MasterList_GatheringItem,
                5 => AppResources.MasterDetail_MasterList_Gadget,
                6 => AppResources.MasterDetail_MasterList_Furnishing,
                7 => AppResources.MasterDetail_MasterList_Gardening,
                8 => AppResources.MasterDetail_MasterList_Talent,
                9 => AppResources.MasterDetail_MasterList_WeaponAscension,
                _ => AppResources.MasterDetail_MasterList_Resin
            };
        }

        private void QuickCalcVibration_OnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.QUICKCALC_VIBRATION, e.Value);
        }

        private async void Notification_OnChanged(object sender, ToggledEventArgs e)
        {
            IBootService bootService = DependencyService.Get<IBootService>();

            Preferences.Set(SettingConstants.NOTI_ENABLED, e.Value);

            if (e.Value)
            {
                if (Device.RuntimePlatform == Device.UWP)
                {
                    if (!await bootService.Register())
                    {
                        string title = AppResources.Bootstrap_ChangeEnableFail_Title;
                        string message = AppResources.Bootstrap_ChangeEnableFail_Message;
                        string ok = AppResources.Dialog_Ok;

                        await DisplayAlert(title, message, ok);
                    }
                }

                DependencyService.Get<IScheduledNoti>().ScheduleAllNoti();
            }
            else
            {
                DependencyService.Get<IScheduledNoti>().CancelAll();

                if (Device.RuntimePlatform == Device.UWP)
                {
                    await bootService.Unregister();
                }
            }
        }

        private void Use24HTimeFormat_OnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.APP_USE_24H_TIMEFORMAT, e.Value);
        }

        //private void SelectVisibleMenu_Tapped(object sender, EventArgs e)
        //{
        //    // Add open BaseDialog - CheckBoxPreferenceView code
        //}

        private async void StartDetailScreen_Tapped(object sender, EventArgs e)
        {
            BaseDialog dialog = new BaseDialog(AppResources.SettingPage_Section_App_Start_DetailScreen_Dialog_Title,
                new RadioPreferenceView(StartScreenList, SettingConstants.APP_START_DETAILSCREEN));

            dialog.OnClose += delegate { StartDetailScreenNow.Text = GetStartScreenString(Preferences.Get(SettingConstants.APP_START_DETAILSCREEN, 0)); };

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void AppLang_Tapped(object sender, EventArgs e)
        {
            BaseDialog dialog = new BaseDialog(AppResources.SettingPage_Section_App_AppLang_Dialog_Title,
                new RadioPreferenceView(AppLangList, SettingConstants.APP_LANG));

            dialog.OnClose += delegate { AppLangNow.Text = AppLangList[Preferences.Get(SettingConstants.APP_LANG, (int)AppEnvironment.AppLang.System)]; };

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void AppInGameServer_Tapped(object sender, EventArgs e)
        {
            BaseDialog dialog = new BaseDialog(AppResources.SettingPage_Section_App_InGameServer_Dialog_Title,
                new RadioPreferenceView(AppEnv.serverList, SettingConstants.APP_INGAMESERVER));

            dialog.OnClose += delegate { AppInGameServerNow.Text = AppEnv.serverList[Preferences.Get(SettingConstants.APP_INGAMESERVER, 0)]; };

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private void ShowOverflow_OnChanged(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.SHOW_OVERFLOW, e.Value);
        }
    }
}