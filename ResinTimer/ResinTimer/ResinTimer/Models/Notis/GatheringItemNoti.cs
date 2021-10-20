using ResinTimer.Resources;

using System;

using GIEnv = ResinTimer.GatheringItemEnvironment;

namespace ResinTimer.Models.Notis
{
    public class GatheringItemNoti : Noti
    {
        public TimeSpan ResetTime { get; set; }
        public GIEnv.GItemType ItemType { get; set; }
        public string ItemNote { get; set; } = string.Empty;

        public string RemainTimeString => $"{((NotiTime >= DateTime.Now) ? $"{GetRemainTimeHM()} {AppResources.GatheringItemTimerPage_Remain}" : AppResources.GatheringItemTimer_ResetComplete)}";
        public string TypeString => ItemType switch
        {
            GIEnv.GItemType.Artifact => AppResources.GatheringItem_Type_Artifact,
            GIEnv.GItemType.Specialty => AppResources.GatheringItem_Type_Specialty,
            GIEnv.GItemType.Artifact12H => AppResources.GatheringItem_Type_Artifact12H,
            GIEnv.GItemType.CrystalChunk => AppResources.GatheringItem_Type_CrystalChunk,
            GIEnv.GItemType.WhiteIronChunk => AppResources.GatheringItem_Type_WhiteIronChunk,
            GIEnv.GItemType.IronChunk => AppResources.GatheringItem_Type_IronChunk,
            GIEnv.GItemType.ElectroCrystal => AppResources.GatheringItem_Type_ElectroCrystal,
            GIEnv.GItemType.CrystalCore => AppResources.GatheringItem_Type_CrystalCore,
            GIEnv.GItemType.AmethystLump => AppResources.GatheringItem_Type_AmethystLump,
            _ => AppResources.GatheringItem_Type_MagicalCrystalChunk
        };
        public string TypeImageName => ItemType switch
        {
            GIEnv.GItemType.Artifact => "artifact24H.png",
            GIEnv.GItemType.Specialty => "silk_flower.png",
            GIEnv.GItemType.Artifact12H => "artifact12H.png",
            GIEnv.GItemType.CrystalChunk => "crystal_chunk.png",
            GIEnv.GItemType.WhiteIronChunk => "white_iron_chunk.png",
            GIEnv.GItemType.IronChunk => "iron_chunk.png",
            GIEnv.GItemType.ElectroCrystal => "electro_crystal.png",
            GIEnv.GItemType.CrystalCore => "crystal_core.png",
            GIEnv.GItemType.AmethystLump => "amethyst_lump.png",
            _ => "magical_crystal_chunk.png"
        };
        public bool ItemNoteVisible => !string.IsNullOrWhiteSpace(ItemNote);

        public GatheringItemNoti(GIEnv.GItemType type = GIEnv.GItemType.MagicCrystalChunk)
        {
            EditItemType(type);

            NotiTime = DateTime.Now;
        }

        public void EditItemType(GIEnv.GItemType type)
        {
            ItemType = type;
            ResetTime = TimeSpan.FromHours(GIEnv.resetTimeList[(int)type]);
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ResetTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_GatheringItem_Title;
        public override string GetNotiText() => $"{TypeString} {AppResources.Noti_GatheringItem_Message}";
    }
}
