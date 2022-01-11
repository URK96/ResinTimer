using ResinTimer.Managers.NotiManagers;

namespace ResinTimer.Models.HomeItems
{
    public class GardeningHomeItem : ListTimerHomeItem
    {
        public override string ImageString => "gardening_jade_field.png";

        public GardeningHomeItem() : base(new GardeningNotiManager()) { }
    }
}
