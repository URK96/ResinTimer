using ResinTimer.Resources;
using ResinTimer.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ResinTimer
{
    public class DailyCheckInEventPage : WebViewPage
    {
        public DailyCheckInEventPage() : base(DailyCheckInService.EVENT_DAILY_CHECKIN_URL)
        {
            Title = AppResources.MasterDetail_MasterList_Event_DailyCheckIn;

#if DEBUG
            var item = new ToolbarItem()
            {
                IconImageSource = "edit.png"
            };
            item.Clicked += delegate 
            { 
                DailyCheckInService.PrintCookie(WebView); 
            };

            ToolbarItems.Add(item);
#endif
        }
    }
}