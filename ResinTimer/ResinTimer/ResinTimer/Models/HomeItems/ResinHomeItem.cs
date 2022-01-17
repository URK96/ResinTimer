using System;

using ResinTimer.Resources;

using REnv = ResinTimer.ResinEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class ResinHomeItem : IHomeItem
    {
        public string StatusMessage => $"{REnv.Resin} / {REnv.MaxResin}";

        public string OptionalMessage => (REnv.EndTime > DateTime.Now) ?
            $"{REnv.TotalCountTime.Hours} : {REnv.TotalCountTime.Minutes:D2} {AppResources.TimerMainPage_Remain}" :
            AppResources.TimerMainPage_Complete;

        public string ImageString => "resin.png";

        public ResinHomeItem()
        {
            REnv.LoadValues();
            REnv.CalcResin();
            REnv.CalcResinTime();
        }
    }
}
