using System;
using System.Collections.Generic;
using System.Threading;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GatheringItemTimerPage : ContentPage
    {
        public List<Noti> Notis => notiManager.Notis;

        private Timer updateTimer;

        private GatheringItemNotiManager notiManager;

        public GatheringItemTimerPage()
        {
            notiManager = new GatheringItemNotiManager();

            InitializeComponent();

            BindingContext = this;

            updateTimer = new Timer(RefreshTime, new AutoResetEvent(false), TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                updateTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                // DependencyService.Get<IToast>().Show(ex.ToString());
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            updateTimer.Change(Timeout.Infinite, Timeout.Infinite);

            //notiManager.SaveNotis();
        }

        private void RefreshCollectionView()
        {
            ListCollectionView.ItemsSource = null;
            ListCollectionView.ItemsSource = Notis;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0:  // Reset Item
                    (ListCollectionView.SelectedItem as GatheringItemNoti).UpdateTime();
                    notiManager.UpdateNotisTime();
                    RefreshCollectionView();
                    break;
                default:
                    break;
            }
        }

        private void RefreshTime(object statusInfo)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(RefreshCollectionView);
            }
            catch (Exception) { }
        }
    }
}