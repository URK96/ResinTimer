using Android.App;
using Android.OS;

using System.Threading.Tasks;

namespace ResinTimer.Droid
{
    [Activity(Label = "@string/AppName", Name = "com.urk.resintimer.SplashActivity", 
              Icon = "@drawable/resintimer_icon", Theme = "@style/ResinTimer.Splash", 
              MainLauncher = true, NoHistory = true, Exported = true)]
    public class SplashActivity : AndroidX.AppCompat.App.AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            CreateNotiChannel();
            _ = RunMainActivity();
        }

        private void CreateNotiChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channelName = Resources.GetString(Resource.String.NotiChannelName);
            var channelDesc = Resources.GetString(Resource.String.NotiChannelDescription);
            var channel = new NotificationChannel(AndroidAppEnvironment.CHANNEL_ID, channelName, NotificationImportance.Default)
            {
                Description = channelDesc
            };

            var notiManager = GetSystemService(NotificationService) as NotificationManager;
            notiManager.CreateNotificationChannel(channel);
        }

        private async Task RunMainActivity()
        {
            await Task.Delay(500);

            StartActivity(typeof(MainActivity));
        }
    }
}