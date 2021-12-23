using ResinTimer.Models;

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
    public partial class BaseMaterialTimerPage : ContentPage
    {
        public List<TalentListItem> Items { get; set; }
        public CollectionView ListView => ListCollectionView;

        public BaseMaterialTimerPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        internal virtual void ShowDetailInfo() { }

        private void ToolbarItemClicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0: // Noti Setting
                    break;
                default:
                    break;
            }
        }

        private void ListCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowDetailInfo();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Utils.RefreshCollectionView(ListCollectionView, Items);
        }
    }
}