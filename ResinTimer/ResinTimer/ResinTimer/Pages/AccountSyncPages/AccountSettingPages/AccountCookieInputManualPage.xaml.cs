using ResinTimer.Resources;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.AccountSyncPages.AccountSettingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountCookieInputManualPage : ContentPage
    {
        public AccountCookieInputManualPage()
        {
            InitializeComponent();
        }

        private async void ApplyButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Utils.SetAccountCookieInfo(LtuidInputEntry.Text, LtokenInputEntry.Text);

                DependencyService.Get<IToast>().Show(AppResources.AccountSetting_Complete);

                await Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                Utils.ShowToast(AppResources.AccountSetting_SetCookieInfo_Fail);
            }
        }

        private void InputEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyButton.IsVisible = !string.IsNullOrWhiteSpace(LtuidInputEntry.Text) &&
                !string.IsNullOrWhiteSpace(LtokenInputEntry.Text);
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