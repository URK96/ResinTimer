using ResinTimer.Managers.NotiManagers;

namespace ResinTimer.Models.HomeItems
{
    public class GIHomeItem : ListTimerHomeItem
    {
        public override string ImageString => "silk_flower.png";

        public GIHomeItem() : base(new GatheringItemNotiManager()) { }
    }
}
