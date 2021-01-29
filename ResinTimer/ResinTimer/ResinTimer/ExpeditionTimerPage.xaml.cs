using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExpeditionTimerPage : ContentPage
    {
        public List<Noti> Notis => notiManager.Notis;
        public ICommand RemoveCommand => new Command<int>((int id) => { RemoveItem(Notis.Find(x => x.NotiId.Equals(id))); });

        private Timer updateTimer;

        private ExpeditionNotiManager notiManager;

        public ExpeditionTimerPage()
        {
            InitializeComponent();

            notiManager = new ExpeditionNotiManager();

            BindingContext = this;

            updateTimer = new Timer(RefreshTime, new AutoResetEvent(false), TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                updateTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1));

                ResetSelection();
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
        }

        private void RefreshCollectionView()
        {
            ListCollectionView.ItemsSource = Array.Empty<Noti>();
            ListCollectionView.ItemsSource = Notis;
        }

        private void ResetSelection()
        {
            ListCollectionView.SelectedItem = null;
            ListCollectionView.SelectedItems = null;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0:  // Reset Item
                    if (ListCollectionView.SelectedItem != null)
                    {
                        (ListCollectionView.SelectedItem as ExpeditionNoti).UpdateTime();
                        notiManager.UpdateNotisTime();
                        RefreshCollectionView();
                    }
                    break;
                case 1: // Edit Item
                    if (ListCollectionView.SelectedItem != null)
                    {
                        await Navigation.PushAsync(new EditExpeditionItemPage(notiManager, NotiManager.EditType.Edit, ListCollectionView.SelectedItem as ExpeditionNoti));
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
                    }
                    break;
                case 2:  // Add Item
                    if (notiManager.Notis.Count >= 5)
                    {
                        DependencyService.Get<IToast>().Show("Expedition limit exceed");
                    }
                    else
                    {
                        await Navigation.PushAsync(new EditExpeditionItemPage(notiManager, NotiManager.EditType.Add));
                    }
                    break;
                case 3:  // Remove Item
                    if (ListCollectionView.SelectedItem != null)
                    {
                        RemoveItem(ListCollectionView.SelectedItem as Noti);
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
                    }
                    break;
                default:
                    break;
            }

            ResetSelection();
        }

        private void RemoveItem(Noti noti)
        {
            notiManager.EditList(noti, NotiManager.EditType.Remove);

            RefreshCollectionView();
        }

        private void RefreshTime(object statusInfo)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(RefreshCollectionView);
            }
            catch (Exception) { }
        }

        private void ListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isVisible = e.CurrentSelection.Count >= 1;

            ResetToolbarItem.IsEnabled = isVisible;
            EditToolbarItem.IsEnabled = isVisible;
            RemoveToolbarItem.IsEnabled = isVisible;
        }
    }
}