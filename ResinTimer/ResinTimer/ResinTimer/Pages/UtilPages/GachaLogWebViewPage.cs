using GenshinInfo.Managers;

using Xamarin.Essentials;
using Xamarin.Forms;

using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer.Pages.UtilPages
{
    public class GachaLogWebViewPage : WebViewPage
    {
        public GachaLogWebViewPage()
        {
            ToolbarItem inputAuthKeyToolbarItem = new()
            {
                IconImageSource = "key.png",
                Priority = 1,
                Order = ToolbarItemOrder.Primary
            };
            inputAuthKeyToolbarItem.Clicked += async delegate { await Navigation.PushAsync(new AuthKeyInputPage()); };

            ToolbarItems.Add(inputAuthKeyToolbarItem);

            GachaInfoManager manager = new();

            manager.SetAuthKey(Preferences.Get(SettingConstants.AUTHKEY_COMMON, string.Empty));

            string url = manager.CreateGachaLogWebViewerUrl(AppEnv.GetLangShortCode);
            
            if (string.IsNullOrEmpty(url))
            {
                Utils.ShowToast("AuthKey is not set");
            }
            else
            {
                NavigateURL(url);
            }
        }
    }
}