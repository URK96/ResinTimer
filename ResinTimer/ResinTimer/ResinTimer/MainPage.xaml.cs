using ResinTimer.Models;
using ResinTimer.Pages.UtilPages;
using ResinTimer.Resources;
using ResinTimer.TimerPages;

using System;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;

namespace ResinTimer
{
    public partial class MainPage : FlyoutPage
    {
        public MainPage()
        {
            InitializeComponent();

            ApplyDetailPage(new(GetStartPage()));
        }

        public MainPage(string startPageId)
        {
            InitializeComponent();

            ApplyDetailPage(new(GetShortcutStartPage(startPageId)));
        }

        private Page GetStartPage()
        {
            Page startPage;

            try
            {
                startPage = Preferences.Get(SettingConstants.APP_START_DETAILSCREEN, 0) switch
                {
                    1 => new ResinTimerPage(),
                    2 => new RealmCurrencyTimerPage(),
                    3 => new RealmFriendshipTimerPage(),
                    4 => new ExpeditionTimerPage(),
                    5 => new GatheringItemTimerPage(),
                    6 => new GadgetTimerPage(),
                    7 => new FurnishingTimerPage(),
                    8 => new GardeningTimerPage(),
                    9 => new TalentItemTimerPage(),
                    10 => new WAItemTimerPage(),
                    _ => new TimerHomePage()
                };
            }
            catch
            {
                startPage = new TimerHomePage();
            }

            return startPage;
        }

        private Page GetShortcutStartPage(string startPageId)
        {
            return startPageId switch
            {
                "app_timer_resin" => new ResinTimerPage(),
                "app_timer_realmcurrency" => new RealmCurrencyTimerPage(),
                "app_timer_realmfriendship" => new RealmFriendshipTimerPage(),
                "app_timer_expedition" => new ExpeditionTimerPage(),
                "app_timer_gatheringitem" => new GatheringItemTimerPage(),
                "app_timer_gadget" => new GadgetTimerPage(),
                "app_timer_furnishing" => new FurnishingTimerPage(),
                "app_timer_gardening" => new GardeningTimerPage(),
                "app_timer_talent" => new TalentItemTimerPage(),
                "app_timer_wa" => new WAItemTimerPage(),
                _ => new TimerHomePage()
            };
        }

        private void MainListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is null)
            {
                return;
            }

            if (e.SelectedItem is MainMasterItem item)
            {
                NavigationPage detailPage = item.Title switch
                {
                    string title when (title == AppResources.MasterDetail_MasterList_Event_DailyCheckIn) &&
                                      !Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_ENABLED, false) =>
                        new(new DailyCheckInEventPage()),

                    _ => new(Activator.CreateInstance(item.Target) as Page)
                };

                ApplyDetailPage(detailPage);

                IsPresented = false;
            }

            (sender as ListView).SelectedItem = null;
        }

        public void ApplyDetailPage(NavigationPage page)
        {
            // TODO Xamarin.Forms.Device.RuntimePlatform(은)는 더 이상 지원되지 않습니다. 대신 Microsoft.Maui.Devices.DeviceInfo.Platform을(를) 사용하세요. 자세한 내용은 https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes(을)를 참조하세요.
            if (Device.RuntimePlatform is Device.UWP)
            {
                page.BarBackgroundColor = Color.FromHex("#0078E8");
            }

            Detail = page;
        }
    }
}
