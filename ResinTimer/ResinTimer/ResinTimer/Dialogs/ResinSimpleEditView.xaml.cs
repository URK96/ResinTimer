using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

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
    public partial class ResinSimpleEditView : ContentView
    {
        public int ResinCount { get; set; }

        public ResinSimpleEditView()
        {
            InitializeComponent();

            ResinUpDown.Maximum = ResinEnvironment.MAX_RESIN;
            ResinUpDown.Minimum = 0;
            ResinUpDown.Value = ResinEnvironment.resin;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var btText = (sender as Button).Text;

            if (btText.Equals(AppResources.Dialog_Ok))
            {
                ResinEnvironment.endTime = ResinEnvironment.endTime.AddSeconds(ResinTime.ONE_RESTORE_INTERVAL * (ResinEnvironment.resin - (double)ResinUpDown.Value));
                ResinEnvironment.lastInputTime = DateTime.Now.ToString();
                ResinEnvironment.CalcResin();
                ResinEnvironment.SaveValue();
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
    }
}