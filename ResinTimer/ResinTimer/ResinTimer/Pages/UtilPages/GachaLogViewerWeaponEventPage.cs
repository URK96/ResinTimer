using ResinTimer.Managers;
using ResinTimer.Resources;

using System.IO;

using static GenshinInfo.Managers.GachaInfoManager;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogViewerWeaponEventPage : GachaLogViewerBasePage
    {
        private readonly string LogSaveFilePath = Path.Combine(FileManager.CacheDirPath, "GachaWeaponEventLastLog.txt");

        public GachaLogViewerWeaponEventPage()
        {
            Title = AppResources.GachaLogViewer_Tab_Weapon;

            gachaListType = GachaTypeNum.WeaponEvent;
            logSaveFilePath = LogSaveFilePath;

            LoadInitLogs();
        }
    }
}