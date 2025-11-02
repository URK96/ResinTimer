using ResinTimer.Resources;

using System;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace ResinTimer.Pages.AccountSyncPages.AccountSettingPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountCautionPage : ContentPage
    {
        public AccountCautionPage()
        {
            InitializeComponent();
        }

        private async void CautionSelectButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button.Text.Equals(AppResources.Dialog_Agree))
            {
                await Navigation.PushAsync(new AccountUIDInputPage());
            }
            else
            {
                await Navigation.PopModalAsync();
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
                button.BackgroundColor = Color.FromArgb("#500682F6");
                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void ButtonReleased(object sender, EventArgs e)
        {
            Button button = sender as Button;

            try
            {
                button.BackgroundColor = Colors.Transparent;
                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }
    }
}