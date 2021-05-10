using Foundation;

using ResinTimer.iOS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;
using GlobalToast;
using Xamarin.Forms;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(ToastiOS))]

namespace ResinTimer.iOS
{
    public class ToastiOS : IToast
    {
        public void Show(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
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
            });
        }
    }
}