using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

using Xamarin.Forms;

namespace ResinTimer.Services
{
    public class DailyCheckInService
    {
        public const string EVENT_DAILY_CHECKIN_URL = "https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481";
        public const string OS_URL = "https://hk4e-api-os.mihoyo.com/event/sol/"; // global
        public const string OS_ACT_ID = "e202102251931481";
        public const string CN_URL = "https://api-takumi.mihoyo.com/event/bbs_sign_reward/"; // chinese
        public const string CN_ACT_ID = "e202009291139501";

        public static void PrintCookie(WebView wv)
        {
            try
            {
                var cookieContainer = wv.Cookies;
                var header = cookieContainer.GetCookieHeader(new Uri("https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481"));
                var cookies = cookieContainer.GetCookies(new Uri("https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481"));

                var json = JsonSerializer.Serialize(cookies, typeof(CookieCollection));
                var jsonCookies = JsonSerializer.Deserialize(json, typeof(List<Cookie>));

                var collection = new CookieCollection();

                var cookieContainerTest = new CookieContainer();
                cookieContainerTest.SetCookies(new Uri("https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481"), header);
                var cookiesTest = cookieContainerTest.GetCookies(new Uri("https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481"));
            }
            catch (Exception ex)
            {

            }
        }

        
    }
}
