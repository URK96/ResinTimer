using ResinTimer.Resources;

using System;

using GEnv = ResinTimer.GadgetEnvironment;

namespace ResinTimer.Models.Notis
{
    public class GadgetNoti : Noti
    {
        public TimeSpan ResetTime { get; set; }
        public GEnv.GadgetType ItemType { get; set; }
        public string ItemNote { get; set; } = string.Empty;

        public string RemainTimeString => $"{((NotiTime >= DateTime.Now) ? $"{GetRemainTimeHM()} {AppResources.GatheringItemTimerPage_Remain}" : AppResources.GadgetTimerPage_ResetComplete)}";
        public string TypeString => ItemType switch
        {
            GEnv.GadgetType.ParametricTransformer => AppResources.Gadget_Type_ParametricTransformer,
            GEnv.GadgetType.PortableWaypoint => AppResources.Gadget_Type_PortableWaypoint,
            _ => AppResources.Gadget_Type_ParametricTransformer
        };
        public string TypeImageName => ItemType switch
        {
            GEnv.GadgetType.ParametricTransformer => "parametric_transformer.png",
            GEnv.GadgetType.PortableWaypoint => "portable_waypoint.png",
            _ => "parametric_transformer.png"
        };
        public bool ItemNoteVisible => !string.IsNullOrWhiteSpace(ItemNote);

        public GadgetNoti(GEnv.GadgetType type = GEnv.GadgetType.ParametricTransformer)
        {
            EditItemType(type);

            NotiTime = DateTime.Now;
        }

        public void EditItemType(GEnv.GadgetType type)
        {
            ItemType = type;
            ResetTime = TimeSpan.FromHours(GEnv.ResetTimeList[(int)type]);
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ResetTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Gadget_Title;
        public override string GetNotiText() => $"{TypeString} {AppResources.Noti_Gadget_Message}";
    }
}
