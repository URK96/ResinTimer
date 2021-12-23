using ResinTimer.Dialogs;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
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
    public class ExpeditionTimerPage : BaseListTimerPage
    {
        public ExpeditionTimerPage() : base()
        {
            Title = AppResources.ExpeditionMain_Title;
            notiManager = new ExpeditionNotiManager();
            Notis = notiManager.Notis;
        }

        internal override async void EditItem()
        {
            base.EditItem();

            if (ListView.SelectedItem != null)
            {
                await Navigation.PushAsync(new EditExpeditionItemPage(notiManager, NotiManager.EditType.Edit, ListView.SelectedItem as ExpeditionNoti));
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
                await Navigation.PushAsync(new EditExpeditionItemPage(notiManager, NotiManager.EditType.Add));
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
    }
}