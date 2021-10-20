using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using GdEnv = ResinTimer.GardeningEnvironment;

using static ResinTimer.AppEnvironment;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;

namespace ResinTimer.TimerPages
{
    public class GardeningTimerPage : BaseListTimerPage
    {
        public GardeningTimerPage() : base()
        {
            Title = AppResources.GardeningTimerPage_Title;
            notiManager = new GardeningNotiManager();
            Notis = notiManager.Notis;
        }

        internal override async void EditItem()
        {
            base.EditItem();

            if (ListView.SelectedItem != null)
            {
                await Navigation.PushAsync(new EditGardeningItemPage(notiManager, NotiManager.EditType.Edit, ListView.SelectedItem as GardeningNoti));
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
            }
        }

        internal override async void AddItem()
        {
            base.AddItem();

            if (notiManager.Notis.Count >= 100)
            {
                DependencyService.Get<IToast>().Show(AppResources.ListTimer_LimitExceed);
            }
            else
            {
                await Navigation.PushAsync(new EditGardeningItemPage(notiManager, NotiManager.EditType.Add));
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

        internal override void OpenEditItemTimeDialog()
        {
            base.OpenEditItemTimeDialog();
        }
    }
}