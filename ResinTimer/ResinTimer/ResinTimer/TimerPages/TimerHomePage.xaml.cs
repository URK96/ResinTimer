using ResinTimer.Helper;
using ResinTimer.Models.HomeItems;
using ResinTimer.Models.Notis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DMEnv = ResinTimer.DailyMissionEnvironment;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerHomePage : ContentPage
    {
        public List<HomeItem> Items { get; }

        public TimerHomePage()
        {
            InitializeComponent();

            Items = new();

            if (DMEnv.IsSyncEnabled)
            {
                Items.Add(new DailyMissionHomeItem());
            }

            Items.AddRange(new HomeItem[]
            {
                new ResinHomeItem(),
                new RealmCurrencyHomeItem(),
                new RealmFriendshipHomeItem(),
                new ExpeditionHomeItem(),
                new GIHomeItem(),
                new GadgetHomeItem(),
                new FurnishingHomeItem(),
                new GardeningHomeItem()
            });

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateItemsInfo();
        }

        private void UpdateItemsInfo()
        {
            SyncHelper.UpdateAll().GetAwaiter().OnCompleted(() =>
            {
                (Items.Find(x => x is ResinHomeItem) as ResinHomeItem)?.UpdateInfo();
                (Items.Find(x => x is RealmCurrencyHomeItem) as RealmCurrencyHomeItem)?.UpdateInfo();
                (Items.Find(x => x is ExpeditionHomeItem) as ExpeditionHomeItem)?.UpdateInfo<ExpeditionNoti>();

                Utils.RefreshCollectionView(ListCollectionView, Items);
            });
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

            if (!item.HasSubMenu)
            {
                return;
            }

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