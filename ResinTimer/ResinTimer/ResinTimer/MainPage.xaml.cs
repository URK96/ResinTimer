using ResinTimer.TimerPages;

using System;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace ResinTimer
{
    public partial class MainPage : FlyoutPage
    {
        public enum TimerPage
        {
            Resin = 0,
            RealmCurrency = 1,
            RealmFriendship,
            Expedition,
            GatheringItem,
            Gadget,
            Furnishing,
            Gardening,
            Talent,
            WeaponAscension
        }

        public MainPage()
        {
            InitializeComponent();

            Detail = new NavigationPage(GetStartPage());
        }

        public MainPage(string startPageId)
        {
            InitializeComponent();

            Detail = new NavigationPage(startPageId switch
            {
                "app_timer_realmcurrency" => new RealmCurrencyTimerPage(),
                "app_timer_realmfriendship" => new RealmFriendshipTimerPage(),
                "app_timer_expedition" => new ExpeditionTimerPage(),
                "app_timer_gatheringitem" => new GatheringItemTimerPage(),
                "app_timer_gadget" => new GadgetTimerPage(),
                "app_timer_furnishing" => new FurnishingTimerPage(),
                "app_timer_gardening" => new GardeningTimerPage(),
                "app_timer_talent" => new TalentTimerPage(),
                "app_timer_wa" => new WeaponAscensionTimerPage(),
                _ => new ResinTimerPage()
            });
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
                    8 => new TalentTimerPage(),
                    9 => new WeaponAscensionTimerPage(),
                    _ => new ResinTimerPage()
                };
            }
            catch
            {
                startPage = new ResinTimerPage();
            }

            return startPage;
        }

        private void MainListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            if (e.SelectedItem is MainMasterItem item)
            {
                Detail = new NavigationPage(Activator.CreateInstance(item.Target) as Page);
                IsPresented = false;
            }

            (sender as ListView).SelectedItem = null;
        }

        private void MainMenuEditButton_Clicked(object sender, EventArgs e)
        {
            //Detail = new NavigationPage(new EditMainFlyoutList());
            //IsPresented = false;
        }
    }
}
