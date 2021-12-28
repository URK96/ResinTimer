using ResinTimer.Resources;

using WAEnv = ResinTimer.WeaponAscensionEnvironment;

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
    }
}