using ResinTimer.Dialogs;
using ResinTimer.Resources;

using Rg.Plugins.Popup.Services;

using System;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppEnv = ResinTimer.AppEnvironment;
using Locations = GenshinDB_Core.GenshinDB.Locations;
using WAEnv = ResinTimer.WeaponAscensionEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeaponAscensionTimerPage : ContentPage
    {
        public WeaponAscensionTimerPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                WAEnv.LoadSettings();
                AppEnv.LoadNowTZInfo();
                //AppEnv.LoadLocationList();
                WAEnv.CheckNowWAItem();
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
                        new RadioPreferenceView(AppEnv.locations.ToArray(), SettingConstants.ITEM_WEAPONASCENSION_LOCATION));
                    locationDialog.OnClose += delegate
                    {
                        WAEnv.Location = (Locations)Preferences.Get(SettingConstants.ITEM_WEAPONASCENSION_LOCATION, 0);

                        WAEnv.CheckNowWAItem();
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
                NowServerLabel.Text = $"{AppResources.NowServer_PreLabel} : {AppEnv.serverList[(int)AppEnv.Server]} ({AppEnv.GetUTCString(AppEnv.serverUTCs[(int)AppEnv.Server])})";
                NowRegionUTCLabel.Text = $"{AppResources.NowUTC_PreLabel} : {AppEnv.TZInfo.DisplayName} ({AppEnv.GetUTCString(AppEnv.TZInfo.BaseUtcOffset.Hours)})";
                NowLocationLabel.Text = $"{AppResources.NowLocation_PreLabel} : {AppEnv.locations[(int)WAEnv.Location]}";
                NowWAItemPreLabel.Text = WAEnv.Item.ItemName.Equals("All") ? AppResources.WeaponAscensionTimerPage_NowWA_PreLabel_All : AppResources.WeaponAscensionTimerPage_NowWA_PreLabel;
                NowWAItemLabel.Text = WAEnv.Item.ItemName.Equals("All") ? "" : AppEnv.genshinDB.FindLangDic(WAEnv.Item.ItemName);
                NowWAItemImage.Source = ImageSource.FromFile(WAEnv.GetWPItemImageName());
            }
            catch { }
        }
    }
}