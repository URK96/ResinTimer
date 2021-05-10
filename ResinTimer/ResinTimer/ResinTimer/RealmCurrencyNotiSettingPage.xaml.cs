using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static ResinTimer.AppEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealmCurrencyNotiSettingPage : ContentPage
    {
        public List<Noti> Notis => notiManager.Notis;
        public ICommand RemoveCommand => new Command<int>((int percentage) => { RemoveItem(percentage); });

        private RealmCurrencyNotiManager notiManager;

        public RealmCurrencyNotiSettingPage()
        {
            InitializeComponent();

            notiManager = new RealmCurrencyNotiManager();

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
                        RemoveItem((ListCollectionView.SelectedItem as RealmCurrencyNoti).Percentage);
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
            var title = AppResources.RealmCurrencyNotiSettingPage_AddDialog_Title;
            var summary = $"{AppResources.RealmCurrencyNotiSettingPage_AddDialog_Summary} (1 ~ 100)";
            var result = await DisplayPromptAsync(title, summary, AppResources.Dialog_Ok, AppResources.Dialog_Cancel, null, -1, Keyboard.Numeric, string.Empty);

            if (result == null)
            {
                return;
            }

            if (int.TryParse(result, out int count))
            {
                if ((count >= 1) &&
                    (count <= 100))
                {
                    notiManager.EditList(new RealmCurrencyNoti(count), NotiManager.EditType.Add);
                    RefreshCollectionView(ListCollectionView, Notis);
                }
                else
                {
                    var title2 = AppResources.NotiSettingPage_OutOfRangeDialog_Title;
                    var summary2 = $"{AppResources.NotiSettingPage_OutOfRangeDialog_Summary} (1 ~ 100)";

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

        private void RemoveItem(int percentage)
        {
            if (Notis.Count > 1)
            {
                notiManager.EditList(new RealmCurrencyNoti(percentage), NotiManager.EditType.Remove);
                RefreshCollectionView(ListCollectionView, Notis);
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_CannotRemoveToast_Message);
            }
        }
    }
}