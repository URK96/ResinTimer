using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using Xamarin.Forms;

using static ResinTimer.AppEnvironment;

namespace ResinTimer.NotiSettingPages
{
    public class ResinNotiSettingPage : BaseNotiSettingPage
    {
        public ResinNotiSettingPage() : base()
        {
            notiManager = new ResinNotiManager();
            Notis = notiManager.Notis;

            RefreshCollectionView(ListView, Notis);
        }

        internal override void RemoveItem(int notiId)
        {
            if (Notis.Count > 1)
            {
                notiManager.EditList(new ResinNoti(notiId), NotiManager.EditType.Remove);

                RefreshCollectionView(ListView, Notis);
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_CannotRemoveToast_Message);
            }
        }

        internal override async void ShowAddItemDialog()
        {
            string title = AppResources.NotiSettingPage_AddDialog_Title;
            string summary = $"{AppResources.NotiSettingPage_AddDialog_Summary} (1 ~ {ResinEnvironment.MAX_RESIN})";
            string result = await DisplayPromptAsync(title, summary, AppResources.Dialog_Ok, AppResources.Dialog_Cancel, null, -1, Keyboard.Numeric, string.Empty);

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

                    RefreshCollectionView(ListView, Notis);
                }
                else
                {
                    string title2 = AppResources.NotiSettingPage_OutOfRangeDialog_Title;
                    string summary2 = $"{AppResources.NotiSettingPage_OutOfRangeDialog_Summary} (1 ~ {ResinEnvironment.MAX_RESIN})";

                    await DisplayAlert(title2, summary2, AppResources.Dialog_Ok);
                }
            }
            else
            {
                string title3 = AppResources.NotiSettingPage_NotIntegerDialog_Title;
                string summary3 = AppResources.NotiSettingPage_NotIntegerDialog_Summary;

                await DisplayAlert(title3, summary3, AppResources.Dialog_Ok);
            }
        }
    }
}