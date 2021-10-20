using ResinTimer.Models.Notis;

using System;

namespace ResinTimer
{
    public interface IScheduledNoti
    {
        void CancelAll();
        void Cancel<T>() where T :Noti;
        void ScheduleAllNoti();
        void Schedule<T>() where T : Noti;
        void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime);
        void TestNoti(string message = "");
    }
}
