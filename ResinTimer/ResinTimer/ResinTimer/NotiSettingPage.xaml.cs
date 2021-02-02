using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotiSettingPage : ContentPage
    {
        public List<Noti> Notis => notiManager.Notis;
        public ICommand RemoveCommand => new Command<int>((int resin) => { RemoveItem(resin); });

        private ResinNotiManager notiManager;

        public NotiSettingPage()
        {
            InitializeComponent();

            notiManager = new ResinNotiManager();

            SetToolbar();

            BindingContext = this;
        }

        private void SetToolbar()
        {
            if (Device.RuntimePlatform != Device.UWP)
            {
                NotiRemoveToolbarItem.IsEnabled = false;
                ToolbarItems.Remove(NotiRemoveToolbarItem);
            }
        }

        private void RefreshCollectionView()
        {
            ListCollectionView.ItemsSource = null;
            ListCollectionView.ItemsSource = Notis;
            ListCollectionView.SelectedItems = null;
            ListCollectionView.SelectedItem = null;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0:  // Add Item
                    ShowAddItemDialog();
                    break;
                case 1:  // Remove Item (Only UWP)
                    if (ListCollectionView.SelectedItem != null)
                    {
                        RemoveItem((ListCollectionView.SelectedItem as ResinNoti).Resin);
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

        private async void ShowAddItemDialog()
        {
            var title = AppResources.NotiSettingPage_AddDialog_Title;
            var summary = $"{AppResources.NotiSettingPage_AddDialog_Summary} (1 ~ {ResinEnvironment.MAX_RESIN})";
            var result = await DisplayPromptAsync(title, summary, AppResources.Dialog_Ok, AppResources.Dialog_Cancel, null, -1, Keyboard.Numeric, string.Empty);

            if (result == null)
            {
                return;
            }

            if (int.TryParse(result, out int count))
            {
                if ((count >= 1) &&
                    (count <= 160))
                {
                    notiManager.EditList(new ResinNoti(count), NotiManager.EditType.Add);
                    RefreshCollectionView();
                }
                else
                {
                    var title2 = AppResources.NotiSettingPage_OutOfRangeDialog_Title;
                    var summary2 = $"{AppResources.NotiSettingPage_OutOfRangeDialog_Summary} (1 ~ {ResinEnvironment.MAX_RESIN})";

                    await DisplayAlert(title2, summary2, AppResources.Dialog_Ok);
                }
            }
            else
            {
                var title3 = AppResources.NotiSettingPage_NotIntegerDialog_Title;
                var summary3 = AppResources.NotiSettingPage_NotIntegerDialog_Summary;

                await DisplayAlert(title3, summary3, AppResources.Dialog_Ok);
            }
        }

        private void RemoveItem(int resin)
        {
            if (Notis.Count > 1)
            {
                notiManager.EditList(new ResinNoti(resin), NotiManager.EditType.Remove);
                RefreshCollectionView();
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_CannotRemoveToast_Message);
            }
        }
    }
}