using ResinTimer.Dialogs;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using static ResinTimer.AppEnvironment;

namespace ResinTimer.TimerPages
{
    public class FurnishingTimerPage : BaseListTimerPage
    {   
        public FurnishingTimerPage() : base()
        {
            Title = AppResources.FurnishingTimerPage_Title;
            notiManager = new FurnishingNotiManager();
            Notis = notiManager.Notis;
        }

        internal override void ResetItem()
        {
            base.ResetItem();

            if (ListView.SelectedItem != null)
            {
                (ListView.SelectedItem as FurnishingNoti).UpdateTime();
                (notiManager as FurnishingNotiManager).UpdateNotisTime();
                RefreshCollectionView(ListView, Notis);
            }
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

                RefreshCollectionView(ListView, Notis);
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
            }
        }

        internal override void EditItemTime()
        {
            base.EditItemTime();
        }
    }
}