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

namespace ResinTimer.TimerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseListTimerPage : ContentPage
    {
        public List<Noti> Notis { get; set; }
        public CollectionView ListView => ListCollectionView;

        private Timer updateTimer;
        public NotiManager notiManager;

        internal virtual void ResetItem() { }
        internal virtual void EditItem() { }
        internal virtual void AddItem() { }
        internal virtual void RemoveItem() { }
        internal virtual async void EditItemTime()
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
                case 1: // Edit Item
                    EditItem();
                    break;
                case 2:  // Add Item
                    AddItem();
                    break;
                case 3:  // Remove Item
                    RemoveItem();
                    break;
                case 4:  // Edit Item Time
                    EditItemTime();
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