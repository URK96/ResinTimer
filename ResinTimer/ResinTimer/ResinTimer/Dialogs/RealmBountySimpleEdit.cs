using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using AppEnv = ResinTimer.AppEnvironment;
using RFEnv = ResinTimer.RealmFriendshipEnvironment;

namespace ResinTimer.Dialogs
{
    public class RealmBountySimpleEdit : ValueSimpleEditView
    {
        private RealmFriendshipNotiManager notiManager;

        public RealmBountySimpleEdit() : base(RFEnv.bounty, 0, RFEnv.MaxRF)
        {
            notiManager = new RealmFriendshipNotiManager();
        }

        internal override void ApplyValue()
        {
            base.ApplyValue();

            var inputValue = int.Parse(SfUpDown.Text);

            RFEnv.lastInputTime = DateTime.Now.ToString(AppEnv.dtCulture);
            RFEnv.bounty = inputValue; //Convert.ToInt32((double)SfUpDown.Value);
            RFEnv.addCount = 0;
             
            RFEnv.CalcRemainTime();
            RFEnv.CalcRF();
            RFEnv.SaveValue();

            notiManager.UpdateNotisTime();
            notiManager.UpdateScheduledNoti<RealmFriendshipNoti>();
        }
    }
}