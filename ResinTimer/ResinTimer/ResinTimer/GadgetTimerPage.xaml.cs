using ResinTimer.Dialogs;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static ResinTimer.AppEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GadgetTimerPage : ContentPage
    {
        public List<Noti> Notis => notiManager.Notis;

        private Timer updateTimer;
        private GadgetNotiManager notiManager;

        public GadgetTimerPage()
        {
            notiManager = new GadgetNotiManager();

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
                case 0:  // Reset Item
                    if (ListCollectionView.SelectedItem != null)
                    {
                        (ListCollectionView.SelectedItem as GadgetNoti).UpdateTime();
                        notiManager.UpdateNotisTime();
                        RefreshCollectionView(ListCollectionView, Notis);
                    }
                    break;
                case 1: // Edit Item
                    if (ListCollectionView.SelectedItem != null)
                    {
                        await Navigation.PushAsync(new EditGadgetItemPage(notiManager, NotiManager.EditType.Edit, ListCollectionView.SelectedItem as GadgetNoti));
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
                    }
                    break;
                case 2:  // Add Item
                    if (notiManager.Notis.Count >= 100)
                    {
                        DependencyService.Get<IToast>().Show("Gadget Item limit exceed");
                    }
                    else
                    {
                        await Navigation.PushAsync(new EditGadgetItemPage(notiManager, NotiManager.EditType.Add));
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
                case 4:  // Edit Time Item
                    if (ListCollectionView.SelectedItem != null)
                    {
                        var dialog = new BaseDialog("Edit Time",
                            new TimeEditView(ListCollectionView.SelectedItem as Noti, notiManager));

                        dialog.OnClose += delegate { RefreshCollectionView(ListCollectionView, Notis); };

                        await PopupNavigation.Instance.PushAsync(dialog);
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

            ResetToolbarItem.IsEnabled = isVisible;
            EditToolbarItem.IsEnabled = isVisible;
            RemoveToolbarItem.IsEnabled = isVisible;
            EditTimeToolbarItem.IsEnabled = isVisible;
        }
    }
}