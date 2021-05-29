using ResinTimer.Dialogs;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Threading;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static ResinTimer.AppEnvironment;

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseListTimerPage : ContentPage
    {
        public List<Noti> Notis { get; set; }
        public CollectionView ListView => ListCollectionView;

        private Timer updateTimer;
        public NotiManager notiManager;

        internal virtual void ResetItem()
        {
            if (ListView.SelectedItem != null)
            {
                (ListView.SelectedItem as Noti).UpdateTime();
                notiManager.UpdateNotisTime();
                RefreshCollectionView(ListView, Notis);
            }
        }

        internal virtual void ResetAllItem()
        {
            foreach (Noti item in ListView.ItemsSource)
            {
                item.UpdateTime();
                notiManager.UpdateNotisTime();
            }

            RefreshCollectionView(ListView, Notis);
        }

        internal virtual void EditItem() { }
        internal virtual void AddItem() { }
        internal virtual void RemoveItem() { }

        internal virtual async void OpenEditItemTimeDialog()
        {
            if (ListView.SelectedItem != null)
            {
                var dialog = new BaseDialog(AppResources.ListTimer_EditTime,
                    new TimeEditView(ListView.SelectedItem as Noti, notiManager));

                dialog.OnClose += delegate { RefreshCollectionView(ListView, Notis); };

                await PopupNavigation.Instance.PushAsync(dialog);
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
            }
        }

        public BaseListTimerPage()
        {
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
                default:
                    break;
            }
        }

        private void RefreshTime(object statusInfo)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() => { RefreshCollectionView(ListCollectionView, Notis); });
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