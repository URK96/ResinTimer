using ResinTimer.Resources;

using System;

using GdEnv = ResinTimer.GardeningEnvironment;

namespace ResinTimer.Models.Notis
{
    public class GardeningNoti : Noti
    {
        public TimeSpan ResetTime { get; set; }
        public GdEnv.FieldType FieldType { get; set; }
        public string ItemNote { get; set; } = string.Empty;

        public string RemainTimeString => $"{((NotiTime >= DateTime.Now) ? $"{GetRemainTimeHM()} {AppResources.GatheringItemTimerPage_Remain}" : AppResources.GardeningItemTimer_Complete)}";
        public string TypeString => FieldType switch
        {
            GdEnv.FieldType.JadeField => AppResources.Gardening_JadeField,
            GdEnv.FieldType.LuxuriantGlebe => AppResources.Gardening_LuxuriantGlebe,
            GdEnv.FieldType.OrderlyMeadow => AppResources.Gardening_OrderlyMeadow,
            _ => AppResources.Gardening_JadeField
        };
        public string TypeImageName => FieldType switch
        {
            GdEnv.FieldType.JadeField => "gardening_jade_field.png",
            GdEnv.FieldType.LuxuriantGlebe => "gardening_luxuriant_glebe.png",
            GdEnv.FieldType.OrderlyMeadow => "gardening_orderly_meadow.png",
            _ => "gardening_jade_field.png"
        };
        public bool ItemNoteVisible => !string.IsNullOrWhiteSpace(ItemNote);

        public GardeningNoti(GdEnv.FieldType type = GdEnv.FieldType.JadeField)
        {
            EditItemType(type);

            NotiTime = DateTime.Now;
        }

        public void EditItemType(GdEnv.FieldType type)
        {
            FieldType = type;
            ResetTime = TimeSpan.FromHours(GdEnv.FULLGROW_INTERVAL);
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ResetTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Gardening_Title;
        public override string GetNotiText() => $"{TypeString} {AppResources.Noti_Gardening_Message}";
    }
}
