using ResinTimer.Managers.NotiManagers;
using ResinTimer.Resources;

using RCEnv = ResinTimer.RealmCurrencyEnvironment;

namespace ResinTimer.Models.Notis
{
    public class RealmCurrencyNoti : Noti
    {
        public int Percentage { get; set; }
        public int Interval { get; set; }
        public int TargetCurrency => RCEnv.MaxRC * Percentage / 100;
        public string NotiValueString => $"{Percentage} % ({TargetCurrency})";

        public RealmCurrencyNoti(int percentage)
        {
            Percentage = percentage;
            NotiId = RealmCurrencyNotiManager.ID_PREINDEX + Percentage;

            try
            {
                UpdateTime();
            }
            catch
            {
                RCEnv.LoadValues();
                UpdateTime();
            }
        }

        private void CalcInterval()
        {
            int remains = RCEnv.MaxRC - TargetCurrency;

            Interval = remains / RCEnv.RCRate;
        }

        public override void UpdateTime()
        {
            CalcInterval();

#if TEST
            NotiTime = RCEnv.endTime.AddMinutes(-Interval);
#else
            NotiTime = RCEnv.endTime.AddHours(-Interval);
#endif
        }

        public override string GetNotiTitle() => AppResources.Noti_RealmCurrency_Message;

        public override string GetNotiText() => $"{Percentage}% {AppResources.Noti_RealmCurrency_Message}";
    }
}
