using ResinTimer.Dialogs;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Locations = GenshinDB_Core.GenshinDB.Locations;
using TalentEnv = ResinTimer.TalentEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TalentTimerPage : ContentPage
    {
        public TalentTimerPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                TalentEnv.LoadSettings();
                TalentEnv.LoadNowTZInfo();
                TalentEnv.LoadLocationList();
                TalentEnv.CheckNowTalentBook();
                RefreshInfo();
            }
            catch { }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var item = sender as ToolbarItem;

            switch (item.Text)
            {
                case "Location":
                    var locationDialog = new BaseDialog(AppResources.TalentTimerPage_SelectLocationDialog_Title, new RadioPreferenceView(TalentEnv.locations.ToArray(), SettingConstants.ITEM_TALENT_LOCATION));
                    locationDialog.OnClose += delegate
                    {
                        TalentEnv.Location = (Locations)Preferences.Get(SettingConstants.ITEM_TALENT_LOCATION, 0);

                        TalentEnv.CheckNowTalentBook();
                        RefreshInfo();
                    };

                    await PopupNavigation.Instance.PushAsync(locationDialog);
                    break;
                case "Server":
                    var serverDialog = new BaseDialog(AppResources.TalentTimerPage_SelectServerDialog_Title, new RadioPreferenceView(TalentEnv.serverList, SettingConstants.ITEM_TALENT_SERVER));
                    serverDialog.OnClose += delegate
                    {
                        TalentEnv.Server = (TalentEnv.Servers)Preferences.Get(SettingConstants.ITEM_TALENT_SERVER, 0);

                        TalentEnv.CheckNowTalentBook();
                        RefreshInfo();
                    };

                    await PopupNavigation.Instance.PushAsync(serverDialog);
                    break;
                default:
                    break;
            }
        }

        private void RefreshInfo()
        {
            try
            {
                NowServerLabel.Text = $"{AppResources.TalentTimerPage_NowServer_PreLabel} : {TalentEnv.serverList[(int)TalentEnv.Server]} ({GetUTCString(TalentEnv.serverUTCs[(int)TalentEnv.Server])})";
                NowRegionUTCLabel.Text = $"{AppResources.TalentTimerPage_NowUTC_PreLabel} : {TalentEnv.TZInfo.DisplayName} ({GetUTCString(TalentEnv.TZInfo.BaseUtcOffset.Hours)})";
                NowLocationLabel.Text = $"{AppResources.TalentTimerPage_NowLocation_PreLabel} : {TalentEnv.locations[(int)TalentEnv.Location]}";
                NowBookPreLabel.Text = TalentEnv.Item.ItemName.Equals("All") ? AppResources.TalentTimerPage_NowBook_PreLabel_All : AppResources.TalentTimerPage_NowBook_PreLabel;
                NowBookLabel.Text = TalentEnv.Item.ItemName.Equals("All") ? "" : AppEnvironment.genshinDB.FindLangDic(TalentEnv.Item.ItemName);
                NowBookImage.Source = ImageSource.FromFile(TalentEnv.GetTalentBookImageName());
            }
            catch { }
        }

        private string GetUTCString(int offset) => $"UTC{((offset >= 0) ? "+" : "")}{offset}";

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var items = new List<string>();

            if (TalentEnv.Item.ItemName.Equals("All"))
            {
                items.AddRange(from item in AppEnvironment.genshinDB.talentItems
                               where item.Location.Equals(TalentEnv.Location)
                               select item.ItemName);
            }
            else
            {
                items.Add(TalentEnv.Item.ItemName);
            }    

            await Navigation.PushAsync(new TalentCharacterPage(items.ToArray()), true);
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