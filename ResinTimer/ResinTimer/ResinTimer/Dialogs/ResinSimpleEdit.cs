using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;

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

            if (int.TryParse(SfUpDown.Text, out int inputValue))
            {
                REnv.endTime = REnv.endTime.AddSeconds(ResinTime.ONE_RESTORE_INTERVAL * (REnv.resin - inputValue));
                REnv.lastInputTime = DateTime.Now.ToString(AppEnv.dtCulture);
                REnv.CalcResin();
                REnv.SaveValue();

                notiManager.UpdateNotisTime();
                notiManager.UpdateScheduledNoti<ResinNoti>();
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.ValueEdit_ValueError_Message);
            }
        }
    }
}