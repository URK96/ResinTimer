using ResinTimer.Resources;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.AccountSyncPages.AccountSettingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountCookieInputPage : ContentPage
    {
        public AccountCookieInputPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }

        private async void InputMethodButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button.Text.Equals(AppResources.AccountSetting_CookieInputMethod_Auto))
            {
                await Navigation.PushAsync(new AccountCookieInputAutoPage()); 
            }
            else if (button.Text.Equals(AppResources.AccountSetting_CookieInputMethod_Manual))
            {
                await Navigation.PushAsync(new AccountCookieInputManualPage());
            }
        }

        private async void ToolbarItemClicked(object sender, EventArgs e)
        {
            // Only have close item

            await Navigation.PopModalAsync();
        }

        private async void ButtonPressed(object sender, EventArgs e)
        {
            Button button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");
                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void ButtonReleased(object sender, EventArgs e)
        {
            Button button = sender as Button;

            try
            {
                button.BackgroundColor = Color.Transparent;
                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }
    }
}