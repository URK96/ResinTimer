using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            RCEnv.lastInputTime = DateTime.Now.ToString(AppEnv.dtCulture);
            RCEnv.currency = Convert.ToInt32((double)SfUpDown.Value);
            RCEnv.addCount = 0;

            RCEnv.CalcRemainTime();
            RCEnv.CalcRC();
            RCEnv.SaveValue();

            notiManager.UpdateNotisTime();
            notiManager.UpdateScheduledNoti<RealmCurrencyNoti>();
        }
    }
}