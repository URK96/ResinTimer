using GenshinInfo.Managers;

using ResinTimer.Pages.AccountSyncPages.AccountSettingPages;
using ResinTimer.Resources;

using System;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.AccountSyncPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountSyncStatusPage : ContentPage
    {
        private bool IsSyncEnabled => Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_ENABLED, false);

        public AccountSyncStatusPage()
        {
            InitializeComponent();

            AccountSyncEnableSwitch.IsToggled = IsSyncEnabled;
            AccountSyncEnableSwitch.Toggled += AccountSyncEnableSwitchToggled;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await CheckAccountSyncStatus();
        }

        private async void ToolbarItemClicked(object sender, EventArgs e)
        {
            // Only have edit item

            await Navigation.PushAsync(new AccountSyncConfigPage());
        }

        private async Task CheckAccountSyncStatus()
        {
            AccountUIDLabel.Text = $"UID : {Utils.UID}";
            AccountSyncDisconnectedTipLabel.IsVisible = false;
            AccountSyncStatusLabel.TextColor = Color.Default;

            if (IsSyncEnabled)
            {
                AccountSyncStatusLabel.Text = AppResources.AccountSyncStatus_Checking;

                await Task.Delay(500);

                bool isConnected = false;

                try
                {
                    GenshinInfoManager manager = new(Utils.UID, Utils.Ltuid, Utils.Ltoken);

                    isConnected = await manager.CheckLogin();
                }
                catch (Exception) { }

                AccountSyncStatusLabel.Text = isConnected ?
                    AppResources.AccountSyncStatus_Connected :
                    AppResources.AccountSyncStatus_Disconnected;
                AccountSyncStatusLabel.TextColor = isConnected ? Color.Green : Color.OrangeRed;
                AccountSyncDisconnectedTipLabel.IsVisible = !isConnected;
            }
            else
            {
                AccountSyncStatusLabel.Text = AppResources.AccountSyncStatus_Disabled;
            }
        }

        private async void AccountSyncEnableSwitchToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set(SettingConstants.APP_ACCOUNTSYNC_ENABLED, e.Value);

            await CheckAccountSyncStatus();
        }

        private async void RenewAccountButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new AccountCautionPage()));
        }

        private async void RemoveAccountButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Utils.ResetAccountInfo();

                Utils.ShowToast(AppResources.AccountSyncStatusPage_RemoveAccount_Success);

                await CheckAccountSyncStatus();
            }
            catch (Exception)
            {
                Utils.ShowToast(AppResources.AccountSyncStatusPage_RemoveAccount_Fail);
            }
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