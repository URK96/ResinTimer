using ResinTimer.Resources;

using WAEnv = ResinTimer.WeaponAscensionEnvironment;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace ResinTimer.TimerPages
{
    public class WAItemTimerPage : BaseMaterialTimerPage
    {
        public WAItemTimerPage()
        {
            Title = AppResources.WAItemTimerPage_Title;
            Items = WAEnv.Instance.Items;
        }

        protected override void OnAppearing()
        {
            WAEnv.Instance.UpdateNowWAItems();

            base.OnAppearing();
        }

        internal override void ShowDetailInfo(object selectedItem)
        {
            DependencyService.Get<IToast>().Show(AppResources.Common_FeatureIsNotReady);
        }
    }
}