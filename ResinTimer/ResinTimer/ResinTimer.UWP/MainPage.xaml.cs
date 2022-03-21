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

            var app = new ResinTimer.App();

            LoadApplication(app);

            //app.SetMainPage(null);
        }
    }
}
