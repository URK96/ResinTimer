using GenshinInfo.Managers;
using GenshinInfo.Models;

using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;

using System.Threading.Tasks;

using ExpEnv = ResinTimer.ExpeditionEnvironment;
using RCEnv = ResinTimer.RealmCurrencyEnvironment;
using REnv = ResinTimer.ResinEnvironment;

namespace ResinTimer.Helper
{
    public static class SyncHelper
    {
        public enum SyncTarget
        {
            Resin = 0,
            RealmCurrency = 1,
            Expedition = 2,
        }

        public static async Task<RTNoteData> GetRTNoteData()
        {
            GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);
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

            if (REnv.SyncServerData(data))
            {
                REnv.SaveValue();
            }

            if (RCEnv.SyncServerData(data))
            {
                RCEnv.SaveValue();
            }

            if (ExpEnv.SyncServerData(data))
            {
                new ExpeditionNotiManager().UpdateScheduledNoti<ExpeditionNoti>();
            }
        }
    }
}
