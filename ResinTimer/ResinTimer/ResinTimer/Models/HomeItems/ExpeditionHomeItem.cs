using ResinTimer.Managers.NotiManagers;

namespace ResinTimer.Models.HomeItems
{
    public class ExpeditionHomeItem : ListTimerHomeItem
    {
        public override string ImageString => "compass.png";

        public ExpeditionHomeItem() : base(new ExpeditionNotiManager()) { }
    }
}
