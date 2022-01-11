using ResinTimer.Resources;

using System;

using FEnv = ResinTimer.FurnishingEnvironment;

namespace ResinTimer.Models.Notis
{
    public class FurnishingNoti : Noti
    {
        public TimeSpan ResetTime { get; set; }
        public FEnv.FurnishType ItemType { get; set; }
        public string ItemNote { get; set; } = string.Empty;

        public string RemainTimeString => $"{((NotiTime >= DateTime.Now) ? $"{GetRemainTimeHM()} {AppResources.GatheringItemTimerPage_Remain}" : AppResources.FurnishingItemTimer_Complete)}";
        public string TypeString => ItemType switch
        {
            FEnv.FurnishType.Rarity3 => AppResources.Furnishing_Rarity_3,
            FEnv.FurnishType.Rarity4 => AppResources.Furnishing_Rarity_4,
            _ => AppResources.Furnishing_Rarity_2
        };
        public string TypeImageName => ItemType switch
        {
            FEnv.FurnishType.Rarity3 => "furnishing_rarity_3.png",
            FEnv.FurnishType.Rarity4 => "furnishing_rarity_4.png",
            _ => "furnishing_rarity_2.png"
        };
        public bool ItemNoteVisible => !string.IsNullOrWhiteSpace(ItemNote);

        public FurnishingNoti(FEnv.FurnishType type = FEnv.FurnishType.Rarity2)
        {
            EditItemType(type);

            NotiTime = DateTime.Now;
        }

        public void EditItemType(FEnv.FurnishType type)
        {
            ItemType = type;
            ResetTime = TimeSpan.FromHours(FEnv.ResetTimeList[(int)type]);
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ResetTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Furnishing_Title;
        public override string GetNotiText() => $"{TypeString} {AppResources.Noti_Furnishing_Message}";
    }
}
