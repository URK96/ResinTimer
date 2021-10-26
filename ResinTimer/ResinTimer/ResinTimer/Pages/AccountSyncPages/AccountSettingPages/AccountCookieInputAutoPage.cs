using System;
using System.Net;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ResinTimer.Pages.AccountSyncPages.AccountSettingPages
{
    public class AccountCookieInputAutoPage : WebViewPage
    {
        const string SiteUrl = "https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481";

        public AccountCookieInputAutoPage() : base(SiteUrl)
        {
            ToolbarItem closeItem = new()
            {
                Text = "Close",
                Priority = 0,
                IconImageSource = "close.png"
            };
            closeItem.Clicked += async delegate
            {
                await Navigation.PopModalAsync();
            };

            ToolbarItem applyItem = new()
            {
                Text = "Apply",
                Priority = 1,
                IconImageSource = "apply.png"
            };
            applyItem.Clicked += async delegate
            {
                await CheckCookies();
            };

            ToolbarItems.Add(closeItem);
            ToolbarItems.Add(applyItem);
        }

        private async Task CheckCookies()
        {
            try
            {
                CookieCollection cookies = WebView.Cookies.GetCookies(new Uri(SiteUrl));

                string ltuid = cookies["ltuid"].Value;
                string ltoken = cookies["ltoken"].Value;

                Utils.SetAccountCookieInfo(ltuid, ltoken);

                await Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().Show("Cannot find account cookie");
            }
        }
    }
}