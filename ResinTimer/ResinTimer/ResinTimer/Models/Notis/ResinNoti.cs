using ResinTimer.Resources;

using System;

namespace ResinTimer.Models.Notis
{
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
}
