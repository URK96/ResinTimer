using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Foundation;
using Windows.UI.Core.Preview;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using System;

namespace ResinTimer.UWP
{
    public sealed partial class MainPage
    {
        public enum CloseAction
        {
            Terminate,
            Systray,
            Consolidate
        }

        public MainPage()
        {
            this.InitializeComponent();

            SystemNavigationManagerPreview mgr = SystemNavigationManagerPreview.GetForCurrentView();
            mgr.CloseRequested += SystemNavigationManager_CloseRequested;

            if (UWPAppEnvironment.toastNotifier == null)
            {
                UWPAppEnvironment.toastNotifier = ToastNotificationManager.CreateToastNotifier();
            }

            var app = new ResinTimer.App();

            LoadApplication(app);

            //app.SetMainPage(null);
        }

        private async void SystemNavigationManager_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            Deferral deferral = e.GetDeferral();

            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }

            e.Handled = false;

            deferral.Complete();
        }
    }
}
