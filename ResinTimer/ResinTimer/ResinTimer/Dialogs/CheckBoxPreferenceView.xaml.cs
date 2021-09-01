using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckBoxPreferenceView : ContentView
    {
        string settingHeaderKey;
        List<string> checkBoxList;

        public CheckBoxPreferenceView(string[] list, string settingHeaderKey)
        {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(settingHeaderKey) ||
                (list.Length < 1))
            {
                return;
            }

            this.settingHeaderKey = settingHeaderKey;
            checkBoxList = new List<string>(list);

            InitList(list);
        }

        private void InitList(string[] list)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                string menu = list[i];

                StackLayout layout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal
                };
                CheckBox cb = new CheckBox
                {
                    IsChecked = Preferences.Get($"{settingHeaderKey}_{menu}", true)
                };
                Label cbText = new Label
                {
                    Text = menu
                };

                layout.Children.Add(cb);
                layout.Children.Add(cbText);
                CheckBoxListContainer.Children.Add(layout);
            }
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