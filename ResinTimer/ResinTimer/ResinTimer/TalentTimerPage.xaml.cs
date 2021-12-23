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
using AppEnv = ResinTimer.AppEnvironment;

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
                //AppEnv.LoadNowTZInfo();
                //AppEnv.LoadLocationList();
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

            switch (item.Priority)
            {
                case 0:  // Location
                    var locationDialog = new BaseDialog(AppResources.TalentTimerPage_SelectLocationDialog_Title, 
                        new RadioPreferenceView(AppEnv.locations.ToArray(), SettingConstants.ITEM_TALENT_LOCATION));
                    locationDialog.OnClose += delegate
                    {
                        TalentEnv.Location = (Locations)Preferences.Get(SettingConstants.ITEM_TALENT_LOCATION, 0);

                        TalentEnv.CheckNowTalentBook();
                        RefreshInfo();
                    };

                    await PopupNavigation.Instance.PushAsync(locationDialog);
                    break;
                default:
                    break;
            }
        }

        private void RefreshInfo()
        {
            try
            {
                NowServerLabel.Text = $"{AppResources.TalentTimerPage_NowServer_PreLabel} : {AppEnv.serverList[(int)AppEnv.Server]} ({AppEnv.GetUTCString(AppEnv.serverUTCs[(int)AppEnv.Server])})";
                NowRegionUTCLabel.Text = $"{AppResources.TalentTimerPage_NowUTC_PreLabel} : {AppEnv.TZInfo.DisplayName} ({AppEnv.GetUTCString(AppEnv.TZInfo.BaseUtcOffset.Hours)})";
                NowLocationLabel.Text = $"{AppResources.TalentTimerPage_NowLocation_PreLabel} : {AppEnv.locations[(int)TalentEnv.Location]}";
                NowBookPreLabel.Text = TalentEnv.Item.ItemName.Equals("All") ? AppResources.TalentTimerPage_NowBook_PreLabel_All : AppResources.TalentTimerPage_NowBook_PreLabel;
                NowBookLabel.Text = TalentEnv.Item.ItemName.Equals("All") ? "" : AppEnv.genshinDB.FindLangDic(TalentEnv.Item.ItemName);
                NowBookImage.Source = ImageSource.FromFile(TalentEnv.GetTalentBookImageName());
            }
            catch { }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var items = new List<string>();

            if (TalentEnv.Item.ItemName.Equals("All"))
            {
                items.AddRange(from item in AppEnv.genshinDB.talentItems
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