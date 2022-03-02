using System;
using System.Threading.Tasks;

using ResinTimer.Resources;

using REnv = ResinTimer.ResinEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class ResinHomeItem : HomeItem
    {
        public override string StatusMessage => $"{REnv.Resin} / {REnv.MaxResin}";

        public override string OptionalMessage => (REnv.EndTime > DateTime.Now) ?
            $"{REnv.TotalCountTime.Hours} : {REnv.TotalCountTime.Minutes:D2} {AppResources.TimerMainPage_Remain}" :
            AppResources.TimerMainPage_Complete;

        public override string ImageString => "resin.png";

        public ResinHomeItem()
        {
            REnv.LoadValues();

            Task.Run(UpdateInfo);
        }

        private async Task UpdateInfo()
        {
            if (REnv.IsSyncEnabled)
            {
                if (await REnv.SyncServerData())
                {
                    REnv.UpdateSaveData();

                    return;
                }
            }

            REnv.CalcResin();
            REnv.CalcResinTime();
        }
    }
}
