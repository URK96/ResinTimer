using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;
using ResinTimer.Resources;

using System;

using Xamarin.Forms;

using AppEnv = ResinTimer.AppEnvironment;
using RFEnv = ResinTimer.RealmFriendshipEnvironment;

namespace ResinTimer.Dialogs
{
    public class RealmFriendshipSimpleEdit : ValueSimpleEditView
    {
        private RealmFriendshipNotiManager notiManager;

        public RealmFriendshipSimpleEdit() : base(RFEnv.Bounty, 0, RFEnv.MaxRF)
        {
            notiManager = new RealmFriendshipNotiManager();
        }

        internal override void ApplyValue()
        {
            base.ApplyValue();

            if (int.TryParse(SfUpDown.Text, out int inputValue))
            {
                RFEnv.LastInputTime = DateTime.Now.ToString(AppEnv.DTCulture);
                RFEnv.Bounty = inputValue; //Convert.ToInt32((double)SfUpDown.Value);
                RFEnv.AddCount = 0;
                
                RFEnv.CalcEndTime();
                RFEnv.CalcRF();
                RFEnv.SaveValue();

                notiManager.UpdateNotisTime();
                notiManager.UpdateScheduledNoti<RealmFriendshipNoti>();
            }
            else
            {
                DependencyService.Get<IToast>().Show(AppResources.ValueEdit_ValueError_Message);
            }
        }
    }
}