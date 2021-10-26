using System;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.AccountSyncPages.AccountSettingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountUIDInputPage : ContentPage
    {
        public AccountUIDInputPage()
        {
            InitializeComponent();

            UIDInputEntry.Text = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_UID, null) ?? string.Empty;
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopModalAsync();

            return base.OnBackButtonPressed();
        }

        private async void ApplyButtonClicked(object sender, EventArgs e)
        {
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_UID, UIDInputEntry.Text);

            await Navigation.PushAsync(new AccountCookieInputPage());
        }

        private void UIDInputEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyButton.IsVisible = !string.IsNullOrWhiteSpace(e.NewTextValue);
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
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