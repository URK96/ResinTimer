using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.NotiSettingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseNotiSettingPage : ContentPage
    {
        public List<Noti> Notis { get; set; }
        public CollectionView ListView => ListCollectionView;

        public NotiManager notiManager;

        public BaseNotiSettingPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        internal virtual void ShowAddItemDialog() { }
        internal virtual void RemoveItem(int notiId) { }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0:  // Add Item
                    ShowAddItemDialog();
                    break;
                case 1:  // Remove Item
                    RemoveItem((ListView.SelectedItem as Noti).NotiId);
                    break;
                default:
                    break;
            }
        }
    }
}