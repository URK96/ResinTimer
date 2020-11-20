using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer
{
    public class ResinTime
    {
        public const int ONE_RESTORE_INTERVAL = 480;

        public int Hour { get; set; }
        public int Min { get; set; }
        public int Sec { get; set; }
        public int TotalSec => (Hour * 3600) + (Min * 60) + Sec;
        public string TimeMinSec => $"{Min} : {Sec:D2}";

        /// <summary>
        /// Use only min < 60 and sec < 60
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="min"></param>
        /// <param name="sec"></param>
        public ResinTime(int hour, int min, int sec)
        {
            Hour = hour;
            Min = min;
            Sec = sec;
        }

        public ResinTime(int sec)
        {
            SetTime(sec);
        }

        public void SetTime(int sec)
        {
            if (sec > 3600)
            {
                Hour = sec / 3600;
                sec %= 3600;
            }
            else
            {
                Hour = 0;
            }

            if (sec > 60)
            {
                Min = sec / 60;
                sec %= 60;
            }
            else
            {
                Min = 0;
            }

            Sec = sec;
        }

        public void SetTime(int hour, int min, int sec)
        {
            Hour = hour;
            Min = min;
            Sec = sec;
        }

        public static (ResinTime, ResinTime) CalcResinTime(DateTime endTime)
        {
            var nowTime = DateTime.Now;
            var total = new ResinTime(0);
            var oneCount = new ResinTime(0);

            var subTime = endTime - nowTime;

            total.SetTime(Convert.ToInt32(subTime.TotalSeconds));

            oneCount.SetTime(total.TotalSec % ONE_RESTORE_INTERVAL);

            return (total, oneCount);
        }
    }
}
