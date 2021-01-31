using ResinTimer.UWP;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

[assembly: Xamarin.Forms.Dependency(typeof(ToastUWP))]

namespace ResinTimer.UWP
{
    public class ToastUWP : IToast
    {
        public void Show(string message)
        {
            var label = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };

            var style = new Style { TargetType = typeof(FlyoutPresenter) };
            style.Setters.Add(new Setter(Control.BackgroundProperty, Windows.UI.Color.FromArgb(200, 6, 130, 246)));
            style.Setters.Add(new Setter(FrameworkElement.MaxHeightProperty, 10));

            var flyout = new Flyout
            {
                Content = label,
                Placement = FlyoutPlacementMode.Full,
                FlyoutPresenterStyle = style
            };

            flyout.ShowAt(Window.Current.Content as FrameworkElement);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += delegate 
            {
                timer.Stop();
                flyout.Hide();
            };
            timer.Start();
        }
    }
}
