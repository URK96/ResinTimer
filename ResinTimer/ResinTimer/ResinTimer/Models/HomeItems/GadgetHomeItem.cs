using ResinTimer.Managers.NotiManagers;

namespace ResinTimer.Models.HomeItems
{
    public class GadgetHomeItem : ListTimerHomeItem
    {
        public override string ImageString => "parametric_transformer.png";

        public GadgetHomeItem() : base(new GadgetNotiManager()) { }
    }
}
