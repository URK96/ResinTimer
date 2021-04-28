using Foundation;

using ResinTimer.iOS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;
using GlobalToast;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ToastiOS))]

namespace ResinTimer.iOS
{
    public class ToastiOS : IToast
    {
        //private NSTimer alertDelay;
        //private UIAlertController alert;

        //public void Show(string message)
        //{
        //    alertDelay = NSTimer.CreateScheduledTimer(2, delegate { DismissToast(); });
        //    alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);

        //    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        //}

        //void DismissToast()
        //{
        //    alert?.DismissViewController(true, null);
        //    alertDelay?.Dispose();
        //}

        public void Show(string message)
        {
            try
            {
                Toast.MakeToast(message)
                    .SetAppearance(new ToastAppearance
                    {
                        Color = (Xamarin.Forms.Application.Current.RequestedTheme == OSAppTheme.Dark) ? UIColor.Black : UIColor.White,
                        MessageColor = (Xamarin.Forms.Application.Current.RequestedTheme == OSAppTheme.Dark) ? UIColor.White : UIColor.Black
                    })
                    .SetShowShadow(true)
                    .Show();
            }
            catch { }
        }
    }
}