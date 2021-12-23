using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using Xamarin.Forms;

using static ResinTimer.AppEnvironment;

namespace ResinTimer.NotiSettingPages
{
    public class RealmCurrencyNotiSettingPage : BaseNotiSettingPage
    {
        public RealmCurrencyNotiSettingPage()
        {
            notiManager = new RealmCurrencyNotiManager();
            Notis = notiManager.Notis;

            Utils.RefreshCollectionView(ListView, Notis);
        }

        internal override async void ShowAddItemDialog()
        {
            string title = AppResources.RealmCurrencyNotiSettingPage_AddDialog_Title;
            string summary = $"{AppResources.RealmCurrencyNotiSettingPage_AddDialog_Summary} (1 ~ 100)";
            string result = await DisplayPromptAsync(title, summary, AppResources.Dialog_Ok, AppResources.Dialog_Cancel, null, -1, Keyboard.Numeric, string.Empty);

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

                    Utils.RefreshCollectionView(ListView, Notis);
                }
                else
                {
                    string title2 = AppResources.NotiSettingPage_OutOfRangeDialog_Title;
                    string summary2 = $"{AppResources.NotiSettingPage_OutOfRangeDialog_Summary} (1 ~ 100)";

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

        internal override void RemoveItem(int notiId)
        {
            if (Notis.Count > 1)
            {
                notiManager.EditList(new RealmCurrencyNoti(notiId - RealmCurrencyNotiManager.ID_PREINDEX), 
                    NotiManager.EditType.Remove);

                Utils.RefreshCollectionView(ListView, Notis);
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.NotiSettingPage_CannotRemoveToast_Message);
            }
        }
    }
}