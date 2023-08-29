using System;

using Rg.Plugins.Popup.Services;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionView : ContentView
    {
        public bool Result { get; private set; } = false;

        public ActionView(string message)
        {
            InitializeComponent();

            MessageLabel.Text = message;
        }

        public ActionView(string message, string positiveText, string negativeText) : this(message)
        {
            PositiveButton.Text = positiveText;
            NegativeButton.Text = negativeText;
        }

        private async void ButtonClicked(object sender, EventArgs e)
        {
            Result = (sender as Button) == PositiveButton;

            await PopupNavigation.Instance.PopAsync();
        }

        private async void ButtonPressed(object sender, EventArgs e)
        {
            Button button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");

                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void ButtonReleased(object sender, EventArgs e)
        {
            Button button = sender as Button;

            try
            {
                button.BackgroundColor = Color.Transparent;

                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }
    }
}