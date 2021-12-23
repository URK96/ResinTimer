using ResinTimer.Dialogs;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Essentials;

using FEnv = ResinTimer.FurnishingEnvironment;

using static ResinTimer.AppEnvironment;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;

namespace ResinTimer.TimerPages
{
    public class FurnishingTimerPage : BaseListTimerPage
    {   
        private ToolbarItem FurnishingSpeedUpToolbarItem { get; set; }

        public FurnishingTimerPage() : base()
        {
            Title = AppResources.FurnishingTimerPage_Title;
            notiManager = new FurnishingNotiManager();
            Notis = notiManager.Notis;

            AddToolbarItem();
        }

        private void AddToolbarItem()
        {
            FurnishingSpeedUpToolbarItem = new ToolbarItem()
            {
                
                Text = AppResources.FurnishingTimerPage_Toolbar_SpeedUp,
                Order = ToolbarItemOrder.Secondary,
                Priority = 10,
            };
            FurnishingSpeedUpToolbarItem.Clicked += FurnishingSpeedUpToolbarItem_Clicked;


            ToolbarItems.Add(FurnishingSpeedUpToolbarItem);
        }

        internal override async void EditItem()
        {
            base.EditItem();

            if (ListView.SelectedItem != null)
            {
                await Navigation.PushAsync(new EditFurnishingItemPage(notiManager, NotiManager.EditType.Edit, ListView.SelectedItem as FurnishingNoti));
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
            }
        }

        internal override async void AddItem()
        {
            base.AddItem();

            if (notiManager.Notis.Count >= 5)
            {
                DependencyService.Get<IToast>().Show(AppResources.ListTimer_LimitExceed);
            }
            else
            {
                await Navigation.PushAsync(new EditFurnishingItemPage(notiManager, NotiManager.EditType.Add));
            }
        }

        internal override void RemoveItem()
        {
            base.RemoveItem();

            if (ListView.SelectedItem != null)
            {
                notiManager.EditList(ListView.SelectedItem as Noti, NotiManager.EditType.Remove);

                Utils.RefreshCollectionView(ListView, Notis);
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
            }
        }

        internal override void OpenEditItemTimeDialog()
        {
            base.OpenEditItemTimeDialog();
        }

        private void FurnishingSpeedUpToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (ListView.SelectedItems.Count < 0)
            {
                return;
            }

            FurnishingNoti selectedNoti = (FurnishingNoti)ListView.SelectedItem;

            selectedNoti.NotiTime = selectedNoti.NotiTime.AddHours(-FEnv.SPEEDUP_HOUR);

            notiManager.EditList(selectedNoti, NotiManager.EditType.EditOnlyTime);

            Utils.RefreshCollectionView(ListView, Notis);
        }

        internal override void ListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            base.ListCollectionView_SelectionChanged(sender, e);

            bool isEnable = e.CurrentSelection.Count >= 1;

            FurnishingSpeedUpToolbarItem.IsEnabled = isEnable;
        }
    }
}