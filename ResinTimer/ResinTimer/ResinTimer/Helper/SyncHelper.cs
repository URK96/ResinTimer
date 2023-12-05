using GenshinInfo.Managers;
using GenshinInfo.Models;

using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;

using System.Threading.Tasks;

using ExpEnv = ResinTimer.ExpeditionEnvironment;
using RCEnv = ResinTimer.RealmCurrencyEnvironment;
using REnv = ResinTimer.ResinEnvironment;
using DMEnv = ResinTimer.DailyMissionEnvironment;

namespace ResinTimer.Helper
{
    public static class SyncHelper
    {
        public enum SyncTarget
        {
            Resin = 0,
            RealmCurrency = 1,
            Expedition = 2,
            DailyMission = 3,
            WeeklyBoss = 4,
        }

        public static async Task<RTNoteData> GetRTNoteData()
        {
            GenshinInfoManager manager = Utils.CreateGenshinInfoManagerInstance();
            RTNoteData data = await manager.GetRealTimeNotes();

            return data;
        }

        public static async Task<bool> Update(SyncTarget target)
        {
            RTNoteData data = await GetRTNoteData();

            return target switch
            {
                SyncTarget.Resin => REnv.SyncServerData(data),
                SyncTarget.RealmCurrency => RCEnv.SyncServerData(data),
                SyncTarget.Expedition => ExpEnv.SyncServerData(data),
                _ => false
            };
        }

        public static async Task UpdateAll()
        {
            RTNoteData data = await GetRTNoteData();

            if (DMEnv.IsSyncEnabled)
            {
                DMEnv.SyncServerData(data);
            }

            if (REnv.IsSyncEnabled && 
                REnv.SyncServerData(data))
            {
                REnv.SaveValue();
            }

            if (RCEnv.IsSyncEnabled &&
                RCEnv.SyncServerData(data))
            {
                RCEnv.SaveValue();
            }

            if (ExpEnv.IsSyncEnabled && 
                ExpEnv.SyncServerData(data))
            {
                new ExpeditionNotiManager().UpdateScheduledNoti<ExpeditionNoti>();
            }
        }
    }
}
