using ResinTimer.Managers.NotiManagers;

namespace ResinTimer.Models.HomeItems
{
    public class FurnishingHomeItem : ListTimerHomeItem
    {
        public override string ImageString => "furnishing_icon.png";

        public FurnishingHomeItem() : base(new FurnishingNotiManager()) { }
    }
}
