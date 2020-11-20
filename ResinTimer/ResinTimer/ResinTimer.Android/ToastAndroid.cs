using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ResinTimer.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ToastAndroid))]

namespace ResinTimer.Droid
{
    public class ToastAndroid : IToast
    {
        public void Show(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }
    }
}