using Android.App;
using Android.OS;

using System.Threading.Tasks;

namespace ResinTimer.Droid
{
    [Activity(Label = "@string/AppName", Icon = "@drawable/resintimer_icon", Theme = "@style/ResinTimer.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AndroidX.AppCompat.App.AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            _ = RunMainActivity();
        }

        private async Task RunMainActivity()
        {
            await Task.Delay(500);

            StartActivity(typeof(MainActivity));
        }
    }
}