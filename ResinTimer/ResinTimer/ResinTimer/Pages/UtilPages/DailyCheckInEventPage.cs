using ResinTimer.Resources;
using ResinTimer.Services;

namespace ResinTimer.Pages.UtilPages
{
    public class DailyCheckInEventPage : WebViewBasePage
    {
        public const string DailyCheckInUrl = 
            "https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481";
        public const string DailyCheckInUrlHonkai3rd =
            "https://act.hoyolab.com/bbs/event/signin-bh3/index.html?act_id=e202110291205111&mhy_auth_required=true&mhy_presentation_style=fullscreen&utm_source=ingame&utm_medium=notice&utm_campaign=pcm";
        public const string DailyCheckInUrlHonkaiStarRail =
            "https://act.hoyolab.com/bbs/event/signin/hkrpg/index.html?act_id=e202303301540311&hyl_auth_required=true&hyl_presentation_style=fullscreen&lang=ko&plat_type=pc";

        public DailyCheckInEventPage() : this(DailyCheckInUrl)
        {
            
        }

        public DailyCheckInEventPage(string url) : base(url)
        {
            Title = AppResources.MasterDetail_MasterList_Event_DailyCheckIn + " (Web)";
        }
    }
}