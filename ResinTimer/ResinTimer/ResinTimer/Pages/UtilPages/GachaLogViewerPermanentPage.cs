using ResinTimer.Resources;

using static GenshinInfo.Managers.GachaInfoManager;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogViewerPermanentPage : GachaLogViewerBasePage
    {
        public GachaLogViewerPermanentPage()
        {
            Title = AppResources.GachaLogViewer_Tab_Permenent;

            gachaListType = GachaTypeNum.Permanent;
        }
    }
}