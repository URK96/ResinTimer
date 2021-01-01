using System;

using Xamarin.Forms;

namespace ResinTimer
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();

            Detail = new NavigationPage(new ResinTimerPage())
            {
                BarBackgroundColor = (Device.RuntimePlatform != Device.UWP) ? Color.FromHex("#3F51B5") : Color.Default
            };
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
