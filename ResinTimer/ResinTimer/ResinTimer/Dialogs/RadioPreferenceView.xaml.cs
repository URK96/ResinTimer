using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RadioPreferenceView : ContentView
    {
        int selectedIndex;
        string settingKey;
        List<string> radioList;

        public RadioPreferenceView(string[] list, string settingKey)
        {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(settingKey) ||
                (list.Length < 1))
            {
                return;
            }

            this.settingKey = settingKey;
            radioList = new List<string>(list);

            InitList(list, Preferences.Get(settingKey, 0));
        }

        private void InitList(string[] list, int initSelected)
        {
            selectedIndex = initSelected;

            for (int i = 0; i < list.Length; ++i)
            {
                var rd = new RadioButton
                {
                    Content = list[i],
                    IsChecked = i.Equals(initSelected)
                };
                rd.CheckedChanged += Rd_CheckedChanged;

                RadioListContainer.Children.Add(rd);
            }
        }

        private void Rd_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                selectedIndex = radioList.FindIndex(x => x.Equals((sender as RadioButton).Content));
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string btText = (sender as Button).Text;

            if (btText.Equals(AppResources.Dialog_Ok))
            {
                Preferences.Set(settingKey, selectedIndex);
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