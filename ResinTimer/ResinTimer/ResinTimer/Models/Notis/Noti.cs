using ResinTimer.Resources;

using System;

namespace ResinTimer.Models.Notis
{
    public abstract class Noti
    {
        public DateTime NotiTime { get; set; }
        public int NotiId { get; set; }
        public bool IsSyncItem { get; set; } = false;

        public string ExpectedNotiTimeString => 
            $"{AppResources.NotiSettingPage_List_ExpectedNotiTime}\n{Utils.GetTimeString(NotiTime)}";

        public abstract void UpdateTime();
        public abstract string GetNotiTitle();
        public abstract string GetNotiText();

        public string GetRemainTimeHM()
        {
            TimeSpan ts = NotiTime - DateTime.Now;

            int hour = ts.Hours + (ts.Days * 24);
            int min = ts.Minutes;

            return $"{hour:D2}:{min:D2}";
        }
    }
}
