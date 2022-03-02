using System;
using System.Threading.Tasks;

using ResinTimer.Resources;

using RCEnv = ResinTimer.RealmCurrencyEnvironment;

namespace ResinTimer.Models.HomeItems
{
    public class RealmCurrencyHomeItem : HomeItem
    {
        public override string StatusMessage => $"{RCEnv.Percentage} %";

        public override string OptionalMessage => (RCEnv.EndTime > DateTime.Now) ?
            $"{(int)RCEnv.TotalCountTime.TotalHours} : {RCEnv.TotalCountTime.Minutes:D2} " +
            $"{AppResources.TimerMainPage_Remain}" :
            AppResources.TimerMainPage_Complete;

        public override string ImageString => "realm_currency.png";

        public RealmCurrencyHomeItem()
        {
            RCEnv.LoadValues();

            Task.Run(UpdateInfo);
        }

        private async Task UpdateInfo()
        {
            if (RCEnv.IsSyncEnabled)
            {
                if (await RCEnv.SyncServerData())
                {
                    RCEnv.UpdateSaveData();

                    return;
                }
            }

            Xamarin.Forms.DependencyService.Get<IToast>().Show("Currency sync fail");

            RCEnv.CalcRC();
        }
    }
}
