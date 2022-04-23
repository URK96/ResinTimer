using ResinTimer.Resources;

using System;

using DMEnv = ResinTimer.DailyMissionEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class DailyMissionHomeItem : HomeItem
    {
        public override string StatusMessage => $"{DMEnv.FinishedCount} / {DMEnv.TotalCount}";

        public override string OptionalMessage => $"{AppResources.DailyMission_ExtraRewardReceived_PreLabel} : " +
            $"{(DMEnv.IsReceiveExtraTaskReward ? "O" : "X")}";

        public override string ImageString => "daily_mission.png";

        public DailyMissionHomeItem()
        {
            HasSubMenu = false;
        }
    }
}
