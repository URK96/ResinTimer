using ResinTimer.Resources;

using System;

using RCEnv = ResinTimer.RealmCurrencyEnvironment;
using RFEnv = ResinTimer.RealmFriendshipEnvironment;
using ExpEnv = ResinTimer.ExpeditionEnvironment;
using GIEnv = ResinTimer.GatheringItemEnvironment;
using GEnv = ResinTimer.GadgetEnvironment;
using FEnv = ResinTimer.FurnishingEnvironment;
using GdEnv = ResinTimer.GardeningEnvironment;
using CLEnv = ResinTimer.ChecklistEnvironment;

namespace ResinTimer
{
    public abstract class Noti
    {
        public DateTime NotiTime { get; set; }
        public int NotiId { get; set; }

        public string ExpectedNotiTimeString => $"{AppResources.NotiSettingPage_List_ExpectedNotiTime}\n{AppEnvironment.GetTimeString(NotiTime)}";

        public abstract void UpdateTime();
        public abstract string GetNotiTitle();
        public abstract string GetNotiText();

        public string GetRemainTimeHM()
        {
            var ts = NotiTime - DateTime.Now;

            int hour = ts.Hours + (ts.Days * 24);
            int min = ts.Minutes;

            return $"{hour:D2}:{min:D2}";
        }
    }

    public class ResinNoti : Noti
    {
        public int Resin { get; set; }
        public int Interval { get; set; }
        public string NotiValueString => Resin.ToString();

        public ResinNoti(int resin)
        {
            NotiId = Resin = resin;
            Interval = (ResinEnvironment.MAX_RESIN - Resin) * ResinTime.ONE_RESTORE_INTERVAL;

            try
            {
                NotiTime = ResinEnvironment.endTime.AddSeconds(-Interval);
            }
            catch (Exception)
            {
                ResinEnvironment.LoadValues();
                NotiTime = ResinEnvironment.endTime.AddSeconds(-Interval);
            }
        }

        public override void UpdateTime()
        {
            NotiTime = ResinEnvironment.endTime.AddSeconds(-Interval);
        }

        public override string GetNotiTitle() => AppResources.NotiTitle;

        public override string GetNotiText() => $"{Resin} {AppResources.NotiText}";
    }

    public class RealmCurrencyNoti : Noti
    {
        public int Percentage { get; set; }
        public int Interval { get; set; }
        public int TargetCurrency => RCEnv.MaxRC * Percentage / 100;
        public string NotiValueString => $"{Percentage} % ({TargetCurrency})";

        public RealmCurrencyNoti(int percentage)
        {
            Percentage = percentage;
            NotiId = RealmCurrencyNotiManager.ID_PREINDEX + Percentage;

            try
            {
                UpdateTime();
            }
            catch
            {
                RCEnv.LoadValues();
                UpdateTime();
            }
        }

        private void CalcInterval()
        {
            int remains = RCEnv.MaxRC - TargetCurrency;

            Interval = remains / RCEnv.RCRate;
        }

        public override void UpdateTime()
        {
            CalcInterval();

#if TEST
            NotiTime = RCEnv.endTime.AddMinutes(-Interval);
#else
            NotiTime = RCEnv.endTime.AddHours(-Interval);
#endif
        }

        public override string GetNotiTitle() => AppResources.Noti_RealmCurrency_Message;

        public override string GetNotiText() => $"{Percentage}% {AppResources.Noti_RealmCurrency_Message}";
    }

    public class RealmFriendshipNoti : Noti
    {
        public int Percentage { get; set; }
        public int Interval { get; set; }
        public int TargetBounty => RFEnv.MaxRF * Percentage / 100;
        public string NotiValueString => $"{Percentage} % ({TargetBounty})";

        public RealmFriendshipNoti(int percentage)
        {
            Percentage = percentage;
            NotiId = RealmFriendshipNotiManager.ID_PREINDEX + Percentage;

            try
            {
                UpdateTime();
            }
            catch
            {
                RFEnv.LoadValues();
                UpdateTime();
            }
        }

        private void CalcInterval()
        {
            int remains = RFEnv.MaxRF - TargetBounty;

            Interval = remains / RFEnv.RFRate;
        }

        public override void UpdateTime()
        {
            CalcInterval();

#if TEST
            NotiTime = RFEnv.endTime.AddMinutes(-Interval);
#else
            NotiTime = RFEnv.endTime.AddHours(-Interval);
#endif
        }

        public override string GetNotiTitle() => AppResources.Noti_RealmFriendship_Title;

        public override string GetNotiText() => $"{Percentage}% {AppResources.Noti_RealmFriendship_Message}";
    }

    public class ExpeditionNoti : Noti
    {
        public TimeSpan ExpeditionTime { get; set; }
        public TimeSpan StandardTime { get; set; }
        public ExpEnv.ExpeditionType ExpeditionType { get; set; }
        public string ItemNote { get; set; } = string.Empty;

        public string RemainTimeString => $"{((NotiTime >= DateTime.Now) ? $"{NotiTime - DateTime.Now:hh\\:mm} {AppResources.ExpeditionTimerPage_Remain}" : AppResources.Expedition_Complete)}";
        public string TypeString => ExpeditionType switch
        {
            ExpEnv.ExpeditionType.Ingredient => AppResources.Expedition_Type_Ingredient,
            ExpEnv.ExpeditionType.Mora => AppResources.Expedition_Type_Mora,
            _ => AppResources.Expedition_Type_Chunk
        };
        public string TypeImageName => ExpeditionType switch
        {
            ExpEnv.ExpeditionType.Ingredient => "ingredient.png",
            ExpEnv.ExpeditionType.Mora => "mora.png",
            _ => "magical_crystal_chunk.png"
        };
        public bool ItemNoteVisible => !string.IsNullOrWhiteSpace(ItemNote);

        public ExpeditionNoti(TimeSpan interval, ExpEnv.ExpeditionType type = ExpEnv.ExpeditionType.Chunk, bool applyTimeEffect = false)
        {
            ExpeditionType = type;

            EditTime(interval, applyTimeEffect);
        }

        public void EditTime(TimeSpan interval, bool applyTimeEffect = false)
        {
            StandardTime = interval;
            ExpeditionTime = TimeSpan.FromHours(interval.Hours * (applyTimeEffect ? 0.75 : 1));

            UpdateTime();
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ExpeditionTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Expedition_Title;
        public override string GetNotiText() => $"'{TypeString} {ExpeditionTime.Hours}H' {AppResources.Noti_Expedition_Message}";
    }

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
            ResetTime = TimeSpan.FromHours(GEnv.resetTimeList[(int)type]);
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ResetTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Gadget_Title;
        public override string GetNotiText() => $"{TypeString} {AppResources.Noti_Gadget_Message}";
    }

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
            ResetTime = TimeSpan.FromHours(FEnv.resetTimeList[(int)type]);
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ResetTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Furnishing_Title;
        public override string GetNotiText() => $"{TypeString} {AppResources.Noti_Furnishing_Message}";
    }

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
            ResetTime = TimeSpan.FromHours(GEnv.resetTimeList[(int)type]);
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ResetTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Gadget_Title;
        public override string GetNotiText() => $"{TypeString} {AppResources.Noti_Gadget_Message}";
    }
}
