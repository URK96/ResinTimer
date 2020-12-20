using Windows.UI.Notifications;

namespace ResinTimer.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            if (UWPAppEnvironment.toastNotifier == null)
            {
                UWPAppEnvironment.toastNotifier = ToastNotificationManager.CreateToastNotifier();
            }

            LoadApplication(new ResinTimer.App());
        }
    }
}
