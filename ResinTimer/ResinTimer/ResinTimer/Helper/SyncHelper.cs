using GenshinInfo.Constants.Indexes;
using GenshinInfo.Managers;
using GenshinInfo.Models;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using AppEnv = ResinTimer.AppEnvironment;
using REnv = ResinTimer.ResinEnvironment;
using RCEnv = ResinTimer.RealmCurrencyEnvironment;

namespace ResinTimer.Helper
{
    public static class SyncHelper
    {
        public enum SyncTarget
        {
            Resin = 0,
            RealmCurrency = 1,
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
        }
    }
}
