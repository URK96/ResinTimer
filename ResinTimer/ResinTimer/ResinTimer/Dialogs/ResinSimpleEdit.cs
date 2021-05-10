using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using AppEnv = ResinTimer.AppEnvironment;
using REnv = ResinTimer.ResinEnvironment;

namespace ResinTimer.Dialogs
{
    public class ResinSimpleEdit : ValueSimpleEditView
    {
        private ResinNotiManager notiManager;

        public ResinSimpleEdit() : base(REnv.resin, 0, REnv.MAX_RESIN)
        {
            notiManager = new ResinNotiManager();
        }

        internal override void ApplyValue()
        {
            base.ApplyValue();

            REnv.endTime = REnv.endTime.AddSeconds(ResinTime.ONE_RESTORE_INTERVAL * (REnv.resin - (double)SfUpDown.Value));
            REnv.lastInputTime = DateTime.Now.ToString(AppEnv.dtCulture);
            REnv.CalcResin();
            REnv.SaveValue();

            notiManager.UpdateNotisTime();
            notiManager.UpdateScheduledNoti<ResinNoti>();
        }
    }
}