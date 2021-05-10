using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using Syncfusion.SfNumericUpDown.XForms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValueSimpleEditView : ContentView
    {
        //public SfNumericUpDown SfUpDown => ValueUpDown;
        public Entry SfUpDown => ValueUpDown;

        int minValue = 0;
        int maxValue = 0;

        internal virtual void ApplyValue() { }

        public ValueSimpleEditView(int initValue, int min, int max)
        {
            InitializeComponent();

            // SfNumericUpDown is crashing on UWP
            // Restore this code when this issue solve

            //ValueUpDown.Maximum = max;
            //ValueUpDown.Minimum = min;
            //ValueUpDown.Value = initValue;

            minValue = min;
            maxValue = max;
            ValueUpDown.Text = initValue.ToString();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var btText = (sender as Button).Text;

            if (btText.Equals(AppResources.Dialog_Ok))
            {
                ApplyValue();
            }

            await PopupNavigation.Instance.PopAsync();
        }

        private async void ButtonPressed(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.FromHex("#500682F6");
                await button.ScaleTo(0.95, 100, Easing.SinInOut);
            }
            catch { }
        }

        private async void ButtonReleased(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.BackgroundColor = Color.Transparent;
                await button.ScaleTo(1.0, 100, Easing.SinInOut);
            }
            catch { }
        }

        private void ValueUpDown_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if (!int.TryParse(e.NewTextValue, out int input))
            {
                entry.Text = e.OldTextValue;
            }
            else
            {
                if (input < minValue)
                {
                    entry.Text = minValue.ToString();
                }
                else if (input > maxValue)
                {
                    entry.Text = maxValue.ToString();
                }
            }
        }
    }
}