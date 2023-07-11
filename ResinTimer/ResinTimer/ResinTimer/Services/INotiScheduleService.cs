using ResinTimer.Models.Notis;

using System;

namespace ResinTimer.Services
{
    public interface INotiScheduleService
    {
        void Cancel<T>() where T : Noti;
        void Schedule<T>() where T : Noti;
        void ScheduleNotiItem<T>(Noti noti) where T : Noti;
        void ScheduleCustomNoti(string title, string message, int id, DateTime notiTime);
        void TestNoti(string message = "");
        void ScheduleAll();
        void CancelAll();
    }
}
