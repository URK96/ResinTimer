using ResinTimer.Models.HomeItems;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerHomePage : ContentPage
    {
        public List<IHomeItem> Items { get; }

        public TimerHomePage()
        {
            InitializeComponent();

            Items = new()
            {
                new ResinHomeItem(),
                new RealmCurrencyHomeItem(),
                new RealmFriendshipHomeItem(),
                new ExpeditionHomeItem()
            };

            BindingContext = this;
        }

        private async void EditToolbarItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditMainFlyoutList());
        }

        private void ListCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}