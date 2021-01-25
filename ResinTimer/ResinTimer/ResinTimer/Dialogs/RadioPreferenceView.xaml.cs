using Rg.Plugins.Popup.Services;

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
    public partial class RadioPreferenceView : ContentView
    {
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

        private async void Rd_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                int index = radioList.FindIndex(x => x.Equals((sender as RadioButton).Content));

                Preferences.Set(settingKey, index);

                await PopupNavigation.Instance.PopAsync();
            }
        }
    }
}