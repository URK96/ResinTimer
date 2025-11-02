using ResinTimer.Resources;

using System;
using System.Windows.Input;
using Microsoft.Maui.Controls.Xaml;

using GUtils = GenshinInfo.Utils;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;

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