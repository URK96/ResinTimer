using ResinTimer.Resources;

using static GenshinInfo.Managers.GachaInfoManager;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogViewerWeaponEventPage : GachaLogViewerBasePage
    {
        public GachaLogViewerWeaponEventPage()
        {
            Title = AppResources.GachaLogViewer_Tab_Weapon;

            gachaListType = GachaTypeNum.WeaponEvent;
        }
    }
}