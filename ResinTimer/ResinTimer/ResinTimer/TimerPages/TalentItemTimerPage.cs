using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using TalentEnv = ResinTimer.TalentEnvironment;

namespace ResinTimer.TimerPages
{
    public class TalentItemTimerPage : BaseMaterialTimerPage
    {
        public TalentItemTimerPage()
        {
            Title = AppResources.TalentItemTimerPage_Title;
            Items = TalentEnv.Instance.Items;
        }

        protected override void OnAppearing()
        {
            TalentEnv.Instance.UpdateNowTalentBooks();

            base.OnAppearing();
        }
    }
}