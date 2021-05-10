using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ResinTimer
{
    public class DailyCheckInEventPage : WebViewPage
    {
        const string EVENT_DAILY_CHECKIN_URL = "https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481";

        public DailyCheckInEventPage() : base(EVENT_DAILY_CHECKIN_URL)
        {
            Title = AppResources.MasterDetail_MasterList_Event_DailyCheckIn;
        }
    }
}