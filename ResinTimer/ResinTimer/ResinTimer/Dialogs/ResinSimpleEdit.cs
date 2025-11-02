using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;

using AppEnv = ResinTimer.AppEnvironment;
using REnv = ResinTimer.ResinEnvironment;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace ResinTimer.Dialogs
{
    public class ResinSimpleEdit : ValueSimpleEditView
    {
        private ResinNotiManager notiManager;

        public ResinSimpleEdit() : base(REnv.Resin, 0, REnv.MaxResin)
        {
            notiManager = new ResinNotiManager();
        }

        internal override void ApplyValue()
        {
            base.ApplyValue();

            if (int.TryParse(SfUpDown.Text, out int inputValue))
            {
                REnv.EndTime = REnv.EndTime.AddSeconds(REnv.OneRestoreInterval * (REnv.Resin - inputValue));
                REnv.LastInputTime = DateTime.Now.ToString(AppEnv.DTCulture);
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