using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using ResinTimer.Resources;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPage : ContentPage
    {
        public InfoPage()
        {
            InitializeComponent();

            VersionLabel.Text = $"v{AppInfo.VersionString} {(AppEnvironment.isDebug ? "Debug" : "Release")}";
        }

        private async void BugReportToolbarItemClicked(object sender, EventArgs e)
        {
            try
            {
                if (await DisplayAlert(AppResources.BugReport_Dialog_Title, AppResources.BugReport_Dialog_Message, AppResources.Dialog_Yes, AppResources.Dialog_No))
                {
                    await Launcher.OpenAsync("https://github.com/URK96/ResinTimer/issues/new");
                }
            }
            catch { }
        }

        private async void OpenSourceLicenseButtonClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new LicensePage(), true);
            }
            catch { }
        }

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");
                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void ButtonReleased(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.Transparent;
                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }
    }
}