using ResinTimer.Helper;
using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;
using Xamarin.Forms;

using ExpEnv = ResinTimer.ExpeditionEnvironment;

namespace ResinTimer.TimerPages
{
    public class ExpeditionTimerPage : BaseListTimerPage
    {
        public ExpeditionTimerPage() : base()
        {
            Title = AppResources.ExpeditionMain_Title;
            NotiManager = new ExpeditionNotiManager();
            Notis = NotiManager.Notis;
        }

        internal override async void EditItem()
        {
            base.EditItem();

            if (ListView.SelectedItem is not null)
            {
                await Navigation.PushAsync(
                    new EditExpeditionItemPage(NotiManager, NotiManager.EditType.Edit, 
                                               ListView.SelectedItem as ExpeditionNoti));
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_NotSelectedToast_Message);
            }
        }

        internal override async void AddItem()
        {
            base.AddItem();

            if (NotiManager.Notis.Count >= 5)
            {
                DependencyService.Get<IToast>().Show(AppResources.ListTimer_LimitExceed);
            }
            else
            {
                await Navigation.PushAsync(new EditExpeditionItemPage(NotiManager, NotiManager.EditType.Add));
            }
        }

        internal override void RemoveItem()
        {
            base.RemoveItem();

            if (ListView.SelectedItem is not null)
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

        internal override void RefreshTime(object statusInfo)
        {
            if (ExpEnv.IsSyncEnabled)
            {
                SyncHelper.Update(SyncHelper.SyncTarget.Expedition).GetAwaiter().OnCompleted(() =>
                {
                    NotiManager.ReloadNotiList<ExpeditionNoti>();

                    base.RefreshTime(statusInfo);
                });
            }
            else
            {
                base.RefreshTime(statusInfo);
            }
        }
    }
}