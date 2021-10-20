using ResinTimer.Managers.NotiManagers;
using ResinTimer.Resources;

using RFEnv = ResinTimer.RealmFriendshipEnvironment;

namespace ResinTimer.Models.Notis
{
    public class RealmFriendshipNoti : Noti
    {
        public int Percentage { get; set; }
        public int Interval { get; set; }
        public int TargetBounty => RFEnv.MaxRF * Percentage / 100;
        public string NotiValueString => $"{Percentage} % ({TargetBounty})";

        public RealmFriendshipNoti(int percentage)
        {
            Percentage = percentage;
            NotiId = RealmFriendshipNotiManager.ID_PREINDEX + Percentage;

            try
            {
                UpdateTime();
            }
            catch
            {
                RFEnv.LoadValues();
                UpdateTime();
            }
        }

        private void CalcInterval()
        {
            int remains = RFEnv.MaxRF - TargetBounty;

            Interval = remains / RFEnv.RFRate;
        }

        public override void UpdateTime()
        {
            CalcInterval();

#if TEST
            NotiTime = RFEnv.endTime.AddMinutes(-Interval);
#else
            NotiTime = RFEnv.endTime.AddHours(-Interval);
#endif
        }

        public override string GetNotiTitle() => AppResources.Noti_RealmFriendship_Title;

        public override string GetNotiText() => $"{Percentage}% {AppResources.Noti_RealmFriendship_Message}";
    }
}
