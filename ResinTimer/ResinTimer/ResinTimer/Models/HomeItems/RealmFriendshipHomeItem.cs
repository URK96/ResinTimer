using ResinTimer.Resources;

using System;

using RFEnv = ResinTimer.RealmFriendshipEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class RealmFriendshipHomeItem : IHomeItem
    {
        public string StatusMessage => $"{RFEnv.Percentage} %";

        public string OptionalMessage => (RFEnv.EndTime > DateTime.Now) ?
            $"{RFEnv.TotalCountTime.Hours} : {RFEnv.TotalCountTime.Minutes:D2} {AppResources.TimerMainPage_Remain}" :
            AppResources.TimerMainPage_Complete;

        public string ImageString => "friendship.png";

        public RealmFriendshipHomeItem()
        {
            RFEnv.LoadValues();
            RFEnv.CalcRF();
        }
    }
}
