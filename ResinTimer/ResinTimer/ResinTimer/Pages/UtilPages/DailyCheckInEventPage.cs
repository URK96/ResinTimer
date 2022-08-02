using ResinTimer.Resources;
using ResinTimer.Services;

namespace ResinTimer.Pages.UtilPages
{
    public class DailyCheckInEventPage : WebViewBasePage
    {
        private const string DailyCheckInUrl = 
            "https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481";

        public DailyCheckInEventPage() : base(DailyCheckInUrl)
        {
            Title = AppResources.MasterDetail_MasterList_Event_DailyCheckIn + " (Web)";
        }
    }
}