﻿using ResinTimer.Resources;

using System;
using System.Net;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ResinTimer.Pages.AccountSyncPages.AccountSettingPages
{
    public class AccountCookieInputAutoPage : WebViewBasePage
    {
        private const string SiteUrl = "https://act.hoyolab.com/ys/event/signin-sea-v3/index.html?act_id=e202102251931481";
        private string _navigateUrl = string.Empty;

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
            applyItem.Clicked += delegate
            {
                RootLayout.Children.Clear();

                WebView wv = new()
                {
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                wv.Cookies = new CookieContainer();
                wv.Navigated += async (sender, e) =>
                {
                    await Task.Delay(4000);
                    await CheckCookies(sender as WebView);
                };
                wv.Navigating += (sender, e) =>
                {
                    _navigateUrl = e.Url;
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
                CookieCollection cookies = wv.Cookies.GetCookies(new Uri(_navigateUrl));

                try
                {
                    string ltuid = cookies["ltuid"].Value;
                    string ltoken = cookies["ltoken"].Value;

                    Utils.SetAccountCookieInfo(ltuid, ltoken);
                    Utils.ResetAccountV2CookieInfo();
                }
                catch { }

                try
                {
                    string ltuidV2 = cookies["ltuid_v2"].Value;
                    string ltokenV2 = cookies["ltoken_v2"].Value;

                    Utils.SetAccountV2CookieInfo(ltuidV2, ltokenV2);
                    Utils.ResetAccountCookieInfo();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

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