using System;
using System.Collections.Generic;
using System.Text;

using RCEnv = ResinTimer.RealmCurrencyEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class RealmCurrencyHomeItem : IHomeItem
    {
        public string StatusMessage => $"{RCEnv.Percentage} %";

        public string OptionalMessage => $"{RCEnv.TotalCountTime.Hours} : {RCEnv.TotalCountTime.Minutes}";

        public string ImageString => "realm_currency.png";

        public RealmCurrencyHomeItem()
        {
            RCEnv.LoadValues();
        }
    }
}
