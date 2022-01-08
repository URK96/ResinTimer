using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer.Models.HomeItems
{
    public interface IHomeItem
    {
        public string StatusMessage { get; }
        public string OptionalMessage { get; }
        public string ImageString { get; }
    }
}
