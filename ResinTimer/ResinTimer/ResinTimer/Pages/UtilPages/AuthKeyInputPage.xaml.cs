using ResinTimer.Resources;

using System;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using GUtils = GenshinInfo.Utils;

namespace ResinTimer.Pages.UtilPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthKeyInputPage : ContentPage
    {
        public ICommand UrlOpenTabCommand => Utils.UrlOpenCommand;

        public AuthKeyInputPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        private async void ToolbarApplyClicked(object sender, EventArgs e)
        {
            try
            {
                string authKeyUrl = AuthKeyUrlInputEntry.Text;
                string authKeyStr = GUtils.ExtractAuthkey(authKeyUrl);

                Preferences.Set(SettingConstants.AUTHKEY_COMMON, authKeyStr);

                Utils.ShowToast(AppResources.AuthKeyInputPage_InputAuthKey_Success);
            }
            catch (Exception)
            {
                Utils.ShowToast(AppResources.AuthKeyInputPage_InputAuthKey_Fail);
            }
            finally
            {
                await Navigation.PopAsync();
            }
        }
    }
}