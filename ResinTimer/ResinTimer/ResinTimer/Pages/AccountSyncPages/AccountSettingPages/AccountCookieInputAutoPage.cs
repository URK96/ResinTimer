using ResinTimer.Resources;

using System;
using System.Net;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ResinTimer.Pages.AccountSyncPages.AccountSettingPages
{
    public class AccountCookieInputAutoPage : WebViewPage
    {
        private const string SiteUrl = "https://webstatic-sea.mihoyo.com/ys/event/signin-sea/index.html?act_id=e202102251931481";

        public AccountCookieInputAutoPage() : base(SiteUrl)
        {
            Title = AppResources.AccountCookieInputAutoPage_Title;

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
                RootLayout.Children.Clear();

                WebView wv = new()
                {
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                wv.Cookies = new CookieContainer();
                wv.Navigated += async (sender, e) =>
                {
                    await Task.Delay(2000);
                    await CheckCookies(sender as WebView);
                };

                RootLayout.Children.Add(wv);

                wv.Source = SiteUrl;
            };

            ToolbarItems.Add(closeItem);
            ToolbarItems.Add(applyItem);
        }

        private async Task CheckCookies(WebView wv)
        {
            try
            {
                CookieCollection cookies = wv.Cookies.GetCookies(new Uri(SiteUrl));

                string ltuid = cookies["ltuid"].Value;
                string ltoken = cookies["ltoken"].Value;

                Utils.SetAccountCookieInfo(ltuid, ltoken);

                Utils.ShowToast(AppResources.AccountSetting_Complete);

                await Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                Utils.ShowToast(AppResources.AccountSetting_SetCookieInfo_Fail);
            }
        }
    }
}