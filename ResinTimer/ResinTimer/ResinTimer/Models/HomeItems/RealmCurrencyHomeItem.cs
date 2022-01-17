using System;

using ResinTimer.Resources;

using RCEnv = ResinTimer.RealmCurrencyEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class RealmCurrencyHomeItem : IHomeItem
    {
        public string StatusMessage => $"{RCEnv.Percentage} %";

        public string OptionalMessage => (RCEnv.EndTime > DateTime.Now) ?
            $"{RCEnv.TotalCountTime.Hours} : {RCEnv.TotalCountTime.Minutes:D2} {AppResources.TimerMainPage_Remain}" :
            AppResources.TimerMainPage_Complete;

        public string ImageString => "realm_currency.png";

        public RealmCurrencyHomeItem()
        {
            RCEnv.LoadValues();
            RCEnv.CalcRC();
        }
    }
}
