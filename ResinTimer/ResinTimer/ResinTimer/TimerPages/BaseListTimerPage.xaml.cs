using ResinTimer.Dialogs;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Threading;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ExpEnv = ResinTimer.ExpeditionEnvironment;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseListTimerPage : ContentPage
    {
        public List<Noti> Notis { get; set; }
        public CollectionView ListView => ListCollectionView;

        internal NotiManager NotiManager;

        private Timer _updateTimer;

        public BaseListTimerPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        private void SetToolbarItem()
        {
            if ((ExpEnv.IsSyncEnabled && (NotiManager is ExpeditionNotiManager)))
            {
                ToolbarItems.Remove(ResetToolbarItem);
                ToolbarItems.Remove(ResetAllToolbarItem);
                ToolbarItems.Remove(EditToolbarItem);
                ToolbarItems.Remove(AddToolbarItem);
                ToolbarItems.Remove(RemoveToolbarItem);
                ToolbarItems.Remove(EditTimeToolbarItem);
            }
            else
            {
                ToolbarItems.Remove(RenewToolbarItem);
            }
        }

        internal virtual void ResetItem()
        {
            if (ListView.SelectedItem is not null)
            {
                (ListView.SelectedItem as Noti).UpdateTime();
                NotiManager.UpdateNotisTime();
                Utils.RefreshCollectionView(ListView, Notis);
            }
        }

        internal virtual void ResetAllItem()
        {
            foreach (Noti item in ListView.ItemsSource)
            {
                item.UpdateTime();
                NotiManager.UpdateNotisTime();
            }

            Utils.RefreshCollectionView(ListView, Notis);
        }

        internal virtual void EditItem() { }
        internal virtual void AddItem() { }
        internal virtual void RemoveItem() { }

        internal virtual async void OpenEditItemTimeDialog()
        {
            if (ListView.SelectedItem is not null)
            {
                BaseDialog dialog = new(AppResources.ListTimer_EditTime,
                    new TimeEditView(ListView.SelectedItem as Noti, NotiManager));

                dialog.OnClose += delegate { Utils.RefreshCollectionView(ListView, Notis); };

                await PopupNavigation.Instance.PushAsync(dialog);
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                SetToolbarItem();

                _updateTimer = new Timer(RefreshTime, new AutoResetEvent(false),
                                        TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1));

                Utils.RefreshCollectionView(ListCollectionView, Notis);
            }
            catch { }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _updateTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _updateTimer?.Dispose();

            NotiManager.SaveNotis();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0:  // Reset Item
                    ResetItem();
                    break;
                case 1:  // Reset All Item
                    ResetAllItem();
                    break;
                case 2: // Edit Item
                    EditItem();
                    break;
                case 3:  // Add Item
                    AddItem();
                    break;
                case 4:  // Remove Item
                    RemoveItem();
                    break;
                case 5:  // Edit Item Time
                    OpenEditItemTimeDialog();
                    break;
                case 6:  // Sync data
                    RefreshTime(null);
                    break;
                default:
                    break;
            }
        }

        internal virtual void RefreshTime(object statusInfo)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() => 
                { 
                    Utils.RefreshCollectionView(ListCollectionView, Notis); 
                });
            }
            catch (Exception) { }
        }

        internal virtual void ListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isEnable = e.CurrentSelection.Count >= 1;

            ResetToolbarItem.IsEnabled = isEnable;
            ResetAllToolbarItem.IsEnabled = Notis.Count != 0;
            EditToolbarItem.IsEnabled = isEnable;
            RemoveToolbarItem.IsEnabled = isEnable;
            EditTimeToolbarItem.IsEnabled = isEnable;
        }
    }
}