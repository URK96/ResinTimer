using ResinTimer.Resources;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.AccountSyncPages.AccountSettingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountCookieInputManualPage : ContentPage
    {
        private enum CookieVersionType
        {
            V1 = 0,
            V2 = 1
        }

        public AccountCookieInputManualPage()
        {
            InitializeComponent();

            foreach (string item in Enum.GetNames(typeof(CookieVersionType)))
            {
                CookieVersionPicker.Items.Add(item);
            }

            CookieVersionPicker.SelectedIndex = (int)CookieVersionType.V2;
        }

        private async void ApplyButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string inputLtuid = LtuidInputEntry.Text;
                string inputLtoken = LtokenInputEntry.Text;
                var cookieVersion = (CookieVersionType)CookieVersionPicker.SelectedIndex;

                switch (cookieVersion)
                {
                    case CookieVersionType.V2:
                        {
                            Utils.SetAccountV2CookieInfo(inputLtuid, inputLtoken);
                        }
                        break;

                    default:
                        {
                            Utils.SetAccountCookieInfo(inputLtuid, inputLtoken);
                        }
                        break;
                }

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

        private void CookieVersionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker)
            {
                var selectedCookieVersion = (CookieVersionType)CookieVersionPicker.SelectedIndex;

                CookieLtuidTitleLabel.Text = selectedCookieVersion switch
                {
                    CookieVersionType.V2 => "ltuid v2",

                    _ => "ltuid"
                };
                CookieLtokenTitleLabel.Text = selectedCookieVersion switch
                {
                    CookieVersionType.V2 => "ltoken v2",

                    _ => "ltoken"
                };
            }
        }
    }
}