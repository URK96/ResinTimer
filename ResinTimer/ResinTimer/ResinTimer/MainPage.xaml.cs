using ResinTimer.Models;
using ResinTimer.TimerPages;

using System;

using Xamarin.Essentials;
using Xamarin.Forms;

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
                    1 => new RealmCurrencyTimerPage(),
                    2 => new RealmFriendshipTimerPage(),
                    3 => new ExpeditionTimerPage(),
                    4 => new GatheringItemTimerPage(),
                    5 => new GadgetTimerPage(),
                    6 => new FurnishingTimerPage(),
                    7 => new GardeningTimerPage(),
                    8 => new TalentItemTimerPage(),
                    9 => new WAItemTimerPage(),
                    _ => new ResinTimerPage()
                };
            }
            catch
            {
                startPage = new ResinTimerPage();
            }

            return startPage;
        }

        private Page GetShortcutStartPage(string startPageId)
        {
            return startPageId switch
            {
                "app_timer_realmcurrency" => new RealmCurrencyTimerPage(),
                "app_timer_realmfriendship" => new RealmFriendshipTimerPage(),
                "app_timer_expedition" => new ExpeditionTimerPage(),
                "app_timer_gatheringitem" => new GatheringItemTimerPage(),
                "app_timer_gadget" => new GadgetTimerPage(),
                "app_timer_furnishing" => new FurnishingTimerPage(),
                "app_timer_gardening" => new GardeningTimerPage(),
                "app_timer_talent" => new TalentItemTimerPage(),
                "app_timer_wa" => new WAItemTimerPage(),
                _ => new ResinTimerPage()
            };
        }

        private void MainListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            if (e.SelectedItem is MainMasterItem item)
            {
                ApplyDetailPage(new(Activator.CreateInstance(item.Target) as Page));

                IsPresented = false;
            }

            (sender as ListView).SelectedItem = null;
        }

        private void ApplyDetailPage(NavigationPage page)
        {
            if (Device.RuntimePlatform is Device.UWP)
            {
                page.BarBackgroundColor = Color.FromHex("#0078E8");
            }

            Detail = page;
        }
    }
}
