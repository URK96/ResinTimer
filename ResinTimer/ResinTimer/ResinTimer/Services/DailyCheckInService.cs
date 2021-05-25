using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Xamarin.Forms;

namespace ResinTimer.Services
{
    public class DailyCheckInService
    {
        public static void PrintCookie(WebView wv)
        {
            try
            {
                var cookieContainer = wv.Cookies;
                var header = cookieContainer.GetCookieHeader(new Uri("https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481"));
                var cookies = cookieContainer.GetCookies(new Uri("https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481"));

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
