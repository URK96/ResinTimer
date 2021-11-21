using ResinTimer.Managers;
using ResinTimer.Resources;

using System.IO;

using static GenshinInfo.Managers.GachaInfoManager;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogViewerPermanentPage : GachaLogViewerBasePage
    {
        private readonly string LogSaveFilePath = Path.Combine(FileManager.CacheDirPath, "GachaPermanentLastLog.txt");

        public GachaLogViewerPermanentPage()
        {
            Title = AppResources.GachaLogViewer_Tab_Permenent;

            gachaListType = GachaTypeNum.Permanent;
            logSaveFilePath = LogSaveFilePath;

            LoadInitLogs();
        }
    }
}