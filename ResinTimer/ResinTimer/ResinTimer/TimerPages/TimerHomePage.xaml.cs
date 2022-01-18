using ResinTimer.Models.HomeItems;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerHomePage : ContentPage
    {
        public List<HomeItem> Items { get; }

        public TimerHomePage()
        {
            InitializeComponent();

            Items = new()
            {
                new ResinHomeItem(),
                new RealmCurrencyHomeItem(),
                new RealmFriendshipHomeItem(),
                new ExpeditionHomeItem(),
                new GIHomeItem(),
                new GadgetHomeItem(),
                new FurnishingHomeItem(),
                new GardeningHomeItem()
            };

            BindingContext = this;
        }

        private async void EditToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditMainFlyoutList());
        }

        private async void ListCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count < 1)
            {
                return;
            }

            var item = e.CurrentSelection.FirstOrDefault() as HomeItem;
            var flyoutPage = Application.Current.MainPage as FlyoutPage;

            await Task.Delay(100);

            flyoutPage.Detail = new NavigationPage(item switch
            {
                ResinHomeItem => new ResinTimerPage(),
                RealmCurrencyHomeItem => new RealmCurrencyTimerPage(),
                RealmFriendshipHomeItem => new RealmFriendshipTimerPage(),
                ExpeditionHomeItem => new ExpeditionTimerPage(),
                GIHomeItem => new GatheringItemTimerPage(),
                GadgetHomeItem => new GadgetTimerPage(),
                FurnishingHomeItem => new FurnishingTimerPage(),
                GardeningHomeItem => new GardeningTimerPage(),
                _ => new ResinTimerPage()
            });
        }
    }
}