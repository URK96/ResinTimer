using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer
{
    public class Noti
    {
        public int Resin { get; set; }
        public int Interval { get; set; }
        public DateTime NotiTime { get; set; }
        public int NotiId { get; set; }

        public string GetExpectedNotiTimeString => $"{AppResources.NotiSettingPage_List_ExpectedNotiTime}{NotiTime}";

        public Noti(int resin)
        {
            NotiId = Resin = resin;
            Interval = (ResinEnvironment.MAX_RESIN - Resin) * ResinTime.ONE_RESTORE_INTERVAL;

            try
            {
                NotiTime = ResinEnvironment.endTime.AddSeconds(-Interval);
            }
            catch
            {
                ResinEnvironment.LoadValues();
                NotiTime = ResinEnvironment.endTime.AddSeconds(-Interval);
            }
        }

        public void UpdateTime()
        {
            NotiTime = ResinEnvironment.endTime.AddSeconds(-Interval);
        }
    }
}
