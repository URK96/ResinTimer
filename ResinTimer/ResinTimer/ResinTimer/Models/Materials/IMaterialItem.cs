using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer.Models.Materials
{
    public interface IMaterialItem
    {
        public string ItemName { get; }
        public string LocationName { get; }
        public string ItemImageString { get; }
    }
}
