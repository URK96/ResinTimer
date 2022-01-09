using System;
using System.Collections.Generic;
using System.Text;

using RFEnv = ResinTimer.RealmFriendshipEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class RealmFriendshipHomeItem : IHomeItem
    {
        public string StatusMessage => $"{RFEnv.Percentage} %";

        public string OptionalMessage => $"{RFEnv.TotalCountTime.Hours} : {RFEnv.TotalCountTime.Minutes}";

        public string ImageString => "friendship.png";

        public RealmFriendshipHomeItem()
        {
            RFEnv.LoadValues();
        }
    }
}
