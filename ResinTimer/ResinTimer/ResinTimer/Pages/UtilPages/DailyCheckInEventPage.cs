using ResinTimer.Resources;
using ResinTimer.Services;

namespace ResinTimer.Pages.UtilPages
{
    public class DailyCheckInEventPage : WebViewBasePage
    {
        public DailyCheckInEventPage() : base(DailyCheckInService.EVENT_DAILY_CHECKIN_URL)
        {
            Title = AppResources.MasterDetail_MasterList_Event_DailyCheckIn + " (Web)";
        }
    }
}