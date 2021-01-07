using ResinTimer.Resources;

using System;

using ExpEnv = ResinTimer.ExpeditionEnvironment;

namespace ResinTimer
{
    public abstract class Noti
    {
        public DateTime NotiTime { get; set; }
        public int NotiId { get; set; }

        public string GetExpectedNotiTimeString => $"{AppResources.NotiSettingPage_List_ExpectedNotiTime}\n{NotiTime}";

        public abstract void UpdateTime();
        public abstract string GetNotiTitle();
        public abstract string GetNotiText();
    }

    public class ResinNoti : Noti
    {
        public int Resin { get; set; }
        public int Interval { get; set; }

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

    public class ExpeditionNoti : Noti
    {
        public TimeSpan ExpeditionTime { get; set; }
        public TimeSpan StandardTime { get; set; }
        public ExpEnv.ExpeditionType ExpeditionType { get; set; }

        public string GetRemainTimeString => $"{((NotiTime >= DateTime.Now) ? $"{NotiTime - DateTime.Now:hh\\:mm} {AppResources.ExpeditionTimerPage_Remain}" : AppResources.Expedition_Complete)}";
        public string GetTypeString => ExpeditionType switch
        {
            ExpEnv.ExpeditionType.Ingredient => AppResources.Expedition_Type_Ingredient,
            ExpEnv.ExpeditionType.Mora => AppResources.Expedition_Type_Mora,
            _ => AppResources.Expedition_Type_Chunk
        };

        public string GetTypeImageName => ExpeditionType switch
        {
            ExpEnv.ExpeditionType.Ingredient => "ingredient.png",
            ExpEnv.ExpeditionType.Mora => "mora.png",
            _ => "chunk.png"
        };

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
        public override string GetNotiText() => $"'{GetTypeString} {ExpeditionTime.Hours}H' {AppResources.Noti_Expedition_Message}";
    }
}
