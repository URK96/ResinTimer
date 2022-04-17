using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using static ResinTimer.AppEnvironment;

namespace ResinTimer.TimerPages
{
    public class GatheringItemTimerPage : BaseListTimerPage
    {
        public GatheringItemTimerPage() : base()
        {
            Title = AppResources.GatheringItemMain_Title;
            NotiManager = new GatheringItemNotiManager();
            Notis = NotiManager.Notis;
        }

        internal override async void EditItem()
        {
            base.EditItem();

            if (ListView.SelectedItem != null)
            {
                await Navigation.PushAsync(new EditGatheringItemPage(NotiManager, NotiManager.EditType.Edit, ListView.SelectedItem as GatheringItemNoti));
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
            }
        }

        internal override async void AddItem()
        {
            base.AddItem();

            if (NotiManager.Notis.Count >= 100)
            {
                DependencyService.Get<IToast>().Show(AppResources.ListTimer_LimitExceed);
            }
            else
            {
                await Navigation.PushAsync(new EditGatheringItemPage(NotiManager, NotiManager.EditType.Add));
            }
        }

        internal override void RemoveItem()
        {
            base.RemoveItem();

            if (ListView.SelectedItem != null)
            {
                NotiManager.EditList(ListView.SelectedItem as Noti, NotiManager.EditType.Remove);

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
    }
}