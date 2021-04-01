using Android.App;
using Android.Widget;

using ResinTimer.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ToastAndroid))]

namespace ResinTimer.Droid
{
    public class ToastAndroid : IToast
    {
        public void Show(string message)
        {
            try
            {
                Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
            }
            catch { }
        }
    }
}