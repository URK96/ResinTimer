using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer
{
    public interface IScheduledNoti
    {
        void CancelAll();
        void ScheduleAllNoti();
    }
}
