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

            Detail = new NavigationPage(GetStartPage());
        }

        public MainPage(string startPageId)
        {
            InitializeComponent();

            Detail = new NavigationPage(startPageId switch
            {
                "app_timer_expedition" => new ExpeditionTimerPage(),
                "app_timer_gatheringitem" => new GatheringItemTimerPage(),
                "app_timer_gadget" => new GadgetTimerPage(),
                "app_timer_furnishing" => new FurnishingTimerPage(),
                "app_timer_talent" => new TalentTimerPage(),
                _ => new ResinTimerPage()
            });
        }

        private Page GetStartPage()
        {
            Page startPage;
            //var bgColor = (Device.RuntimePlatform != Device.UWP) ? Color.FromHex("#3F51B5") : Color.Default;

            try
            {
                startPage = Preferences.Get(SettingConstants.APP_START_DETAILSCREEN, 0) switch
                {
                    1 => new ExpeditionTimerPage(),
                    2 => new GatheringItemTimerPage(),
                    3 => new GadgetTimerPage(),
                    4 => new FurnishingTimerPage(),
                    5 => new TalentTimerPage(),
                    6 => new WeaponAscensionTimerPage(),
                    _ => new ResinTimerPage()
                };
            }
            catch
            {
                startPage = new ResinTimerPage();
            }

            //startPage.BackgroundColor = bgColor;

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
    }
}
