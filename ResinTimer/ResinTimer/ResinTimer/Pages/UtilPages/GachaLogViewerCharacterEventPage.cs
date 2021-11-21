using ResinTimer.Managers;
using ResinTimer.Resources;

using System.IO;

using static GenshinInfo.Managers.GachaInfoManager;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogViewerCharacterEventPage : GachaLogViewerBasePage
    {
        private readonly string LogSaveFilePath = Path.Combine(FileManager.CacheDirPath, "GachaCharacterEventLastLog.txt");

        public GachaLogViewerCharacterEventPage()
        {
            Title = AppResources.GachaLogViewer_Tab_Character;

            gachaListType = GachaTypeNum.CharacterEvent;
            logSaveFilePath = LogSaveFilePath;

            LoadInitLogs();
        }
    }
}