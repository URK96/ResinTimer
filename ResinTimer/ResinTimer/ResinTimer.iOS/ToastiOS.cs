using Foundation;

using ResinTimer.iOS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(ToastiOS))]

namespace ResinTimer.iOS
{
    public class ToastiOS : IToast
    {
        private NSTimer alertDelay;
        private UIAlertController alert;

        public void Show(string message)
        {
            alertDelay = NSTimer.CreateScheduledTimer(2, delegate { DismissToast(); });
            alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        }

        void DismissToast()
        {
            alert?.DismissViewController(true, null);
            alertDelay?.Dispose();
        }
    }
}