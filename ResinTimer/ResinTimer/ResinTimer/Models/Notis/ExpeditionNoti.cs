using System;
using System.Net.Http;

using ResinTimer.Resources;

using AppEnv = ResinTimer.AppEnvironment;
using ExpEnv = ResinTimer.ExpeditionEnvironment;

namespace ResinTimer.Models.Notis
{
    public class ExpeditionNoti : Noti
    {
        public TimeSpan ExpeditionTime { get; set; }
        public TimeSpan StandardTime { get; set; }
        public ExpEnv.ExpeditionType ExpeditionType { get; set; }
        public string ItemNote { get; set; } = string.Empty;
        public string SyncModeCharacter { get; set; } = string.Empty;

        public string SyncModeImage { get; set; } = string.Empty;

        public string RemainTimeString => (NotiTime >= DateTime.Now) ?
            $"{NotiTime - DateTime.Now:hh\\:mm} {AppResources.ExpeditionTimerPage_Remain}" :
            AppResources.Expedition_Complete;
        public string TypeString => ExpeditionType switch
        {
            ExpEnv.ExpeditionType.Sync => 
                $"(Sync) {(AppEnv.GDB ??= new(AppResources.Culture)).FindLangDic(SyncModeCharacter)}",
            ExpEnv.ExpeditionType.Ingredient => AppResources.Expedition_Type_Ingredient,
            ExpEnv.ExpeditionType.Mora => AppResources.Expedition_Type_Mora,
            _ => AppResources.Expedition_Type_Chunk
        };
        public string TypeImageName => ExpeditionType switch
        {
            ExpEnv.ExpeditionType.Sync => SyncModeImage,
            ExpEnv.ExpeditionType.Ingredient => "ingredient.png",
            ExpEnv.ExpeditionType.Mora => "mora.png",
            _ => "magical_crystal_chunk.png"
        };
        public bool ItemNoteVisible => !string.IsNullOrWhiteSpace(ItemNote);

        public ExpeditionNoti(TimeSpan interval, ExpEnv.ExpeditionType type = ExpEnv.ExpeditionType.Chunk,
                              bool applyTimeEffect = false)
        {
            ExpeditionType = type;
            IsSyncItem = type is ExpEnv.ExpeditionType.Sync;

            EditTime(interval, applyTimeEffect);
        }

        public void EditTime(TimeSpan interval, bool applyTimeEffect = false)
        {
            StandardTime = interval;
            ExpeditionTime = TimeSpan.FromSeconds(interval.TotalSeconds * (applyTimeEffect ? 0.75 : 1));

            UpdateTime();
        }

        public override void UpdateTime()
        {
            NotiTime = DateTime.Now.AddSeconds(ExpeditionTime.TotalSeconds);
        }

        public override string GetNotiTitle() => AppResources.Noti_Expedition_Title;

        public override string GetNotiText() => $"'{TypeString} {ExpeditionTime.Hours}H' " +
            $"{AppResources.Noti_Expedition_Message}";

        public override byte[] GetIconData()
        {
            byte[] data = Array.Empty<byte>();

            if (!string.IsNullOrEmpty(SyncModeImage))
            {
                using HttpClient client = new();

                data = client.GetByteArrayAsync(SyncModeImage)
                             .GetAwaiter()
                             .GetResult();
            }

            return data;
        }
    }
}
