using ResinTimer.Resources;

using System;

using REnv = ResinTimer.ResinEnvironment;

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
            Interval = (REnv.MaxResin - Resin) * REnv.OneRestoreInterval;

            try
            {
                NotiTime = REnv.EndTime.AddSeconds(-Interval);
            }
            catch (Exception)
            {
                REnv.LoadValues();

                NotiTime = REnv.EndTime.AddSeconds(-Interval);
            }
        }

        public override void UpdateTime()
        {
            NotiTime = REnv.EndTime.AddSeconds(-Interval);
        }

        public override string GetNotiTitle() => AppResources.NotiTitle;

        public override string GetNotiText() => $"{Resin} {AppResources.NotiText}";
    }
}
