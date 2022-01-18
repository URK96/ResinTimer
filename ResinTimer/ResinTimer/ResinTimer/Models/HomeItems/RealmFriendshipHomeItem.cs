using ResinTimer.Resources;

using System;

using RFEnv = ResinTimer.RealmFriendshipEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class RealmFriendshipHomeItem : HomeItem
    {
        public override string StatusMessage => $"{RFEnv.Percentage} %";

        public override string OptionalMessage => (RFEnv.EndTime > DateTime.Now) ?
            $"{(int)RFEnv.TotalCountTime.TotalHours} : {RFEnv.TotalCountTime.Minutes:D2} " +
            $"{AppResources.TimerMainPage_Remain}" :
            AppResources.TimerMainPage_Complete;

        public override string ImageString => "friendship.png";

        public RealmFriendshipHomeItem()
        {
            RFEnv.LoadValues();
            RFEnv.CalcRF();
        }
    }
}
