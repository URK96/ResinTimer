using System;
using System.Collections.Generic;
using System.Text;

using REnv = ResinTimer.ResinEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class ResinHomeItem : IHomeItem
    {
        public string StatusMessage => $"{REnv.resin} / {REnv.MAX_RESIN}";

        public string OptionalMessage => throw new NotImplementedException();

        public string ImageString => "resin.png";
    }
}
