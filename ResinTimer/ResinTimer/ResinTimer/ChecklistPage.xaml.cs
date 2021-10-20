using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Threading;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static ResinTimer.AppEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChecklistPage : ContentPage
    {
        public List<Noti> Notis => notiManager.Notis;

        private Timer updateTimer;
        private ChecklistNotiManager notiManager;

        public ChecklistPage()
        {
            notiManager = new ChecklistNotiManager();

            InitializeComponent();

            BindingContext = this;

            updateTimer = new Timer(RefreshTime, new AutoResetEvent(false), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                updateTimer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1));

                RefreshCollectionView(ListCollectionView, Notis);
            }
            catch { }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            updateTimer.Change(Timeout.Infinite, Timeout.Infinite);

            notiManager.SaveNotis();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0: // Edit Item
                    if (ListCollectionView.SelectedItem != null)
                    {
                        await Navigation.PushAsync(new EditGadgetItemPage(notiManager, NotiManager.EditType.Edit, ListCollectionView.SelectedItem as GadgetNoti));
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
                    }
                    break;
                case 1:  // Add Item
                    if (notiManager.Notis.Count >= 100)
                    {
                        DependencyService.Get<IToast>().Show("Checklist Item limit exceed");
                    }
                    else
                    {
                        await Navigation.PushAsync(new EditGadgetItemPage(notiManager, NotiManager.EditType.Add));
                    }
                    break;
                case 2:  // Remove Item
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
        }

        private void RemoveItem(Noti noti)
        {
            notiManager.EditList(noti, NotiManager.EditType.Remove);

            RefreshCollectionView(ListCollectionView, Notis);
        }

        private void RefreshTime(object statusInfo)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() => { RefreshCollectionView(ListCollectionView, Notis); });
            }
            catch (Exception) { }
        }

        private void ListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isVisible = e.CurrentSelection.Count >= 1;

            EditToolbarItem.IsEnabled = isVisible;
            RemoveToolbarItem.IsEnabled = isVisible;
        }
    }
}