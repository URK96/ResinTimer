using ResinTimer.Resources;

using static GenshinInfo.Managers.GachaInfoManager;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogViewerCharacterEventPage : GachaLogViewerBasePage
    {
        public GachaLogViewerCharacterEventPage()
        {
            Title = AppResources.GachaLogViewer_Tab_Character;

            gachaListType = GachaTypeNum.CharacterEvent;
        }
    }
}