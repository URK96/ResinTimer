using ResinTimer.Resources;

using System;

using CLEnv = ResinTimer.ChecklistEnvironment;

namespace ResinTimer.Models.Notis
{
    // This class is under working
    public class ChecklistNoti : Noti
    {
        public CLEnv.ResetType ItemType { get; set; }
        public TimeSpan ResetTime { get; set; }
        public string ItemNote { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        public string RemainTimeString => $"{((NotiTime >= DateTime.Now) ? $"{GetRemainTimeHM()} {AppResources.GatheringItemTimerPage_Remain}" : AppResources.GatheringItemTimer_ResetComplete)}";
        public string TypeString => ItemType switch
        {
            CLEnv.ResetType.Once => AppResources.Checklist_Type_Once,
            CLEnv.ResetType.Custom => AppResources.Checklist_Type_Custom,
            CLEnv.ResetType.Daily => AppResources.Checklist_Type_Daily,
            CLEnv.ResetType.Weekly => AppResources.Checklist_Type_Weekly,
            CLEnv.ResetType.Monthly => AppResources.Checklist_Type_Monthly,
            _ => AppResources.Checklist_Type_Once
        };
        public bool ItemNoteVisible => !string.IsNullOrWhiteSpace(ItemNote);

        public ChecklistNoti(CLEnv.ResetType type = CLEnv.ResetType.Once)
        {
            EditItemType(type);

            NotiTime = DateTime.Now;
        }

        public void EditItemType(CLEnv.ResetType type)
        {
            ItemType = type;
            //ResetTime = TimeSpan.FromHours(GEnv.resetTimeList[(int)type]);
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ResetTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Gadget_Title;
        public override string GetNotiText() => $"{TypeString} {AppResources.Noti_Gadget_Message}";
    }
}
