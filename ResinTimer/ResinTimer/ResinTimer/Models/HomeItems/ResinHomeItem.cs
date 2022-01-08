using System;
using System.Collections.Generic;
using System.Text;

using REnv = ResinTimer.ResinEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class ResinHomeItem : IHomeItem
    {
        public string StatusMessage => $"{REnv.Resin} / {REnv.MaxResin}";

        public string OptionalMessage => REnv.TotalCountTime.ToString();

        public string ImageString => "resin.png";
    }
}
