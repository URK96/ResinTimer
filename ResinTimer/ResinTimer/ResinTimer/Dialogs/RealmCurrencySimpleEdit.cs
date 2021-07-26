using ResinTimer.Resources;

using System;

using Xamarin.Forms;

using AppEnv = ResinTimer.AppEnvironment;
using RCEnv = ResinTimer.RealmCurrencyEnvironment;

namespace ResinTimer.Dialogs
{
    public class RealmCurrencySimpleEdit : ValueSimpleEditView
    {
        private RealmCurrencyNotiManager notiManager;

        public RealmCurrencySimpleEdit() : base(RCEnv.currency, 0, RCEnv.MaxRC)
        {
            notiManager = new RealmCurrencyNotiManager();
        }

        internal override void ApplyValue()
        {
            base.ApplyValue();

            if (int.TryParse(SfUpDown.Text, out int inputValue))
            {
                RCEnv.lastInputTime = DateTime.Now.ToString(AppEnv.dtCulture);
                RCEnv.currency = inputValue; //Convert.ToInt32((double)SfUpDown.Value);
                RCEnv.addCount = 0;

                RCEnv.CalcRemainTime();
                RCEnv.CalcRC();
                RCEnv.SaveValue();

                notiManager.UpdateNotisTime();
                notiManager.UpdateScheduledNoti<RealmCurrencyNoti>();
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.ValueEdit_ValueError_Message);
            }
        }
    }
}