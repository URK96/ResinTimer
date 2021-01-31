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
using TalentItem = GenshinDB_Core.TalentItem;
using TalentEnv = ResinTimer.TalentEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TalentTimerPage : ContentPage
    {
        private TalentEnv.Servers nowServer;
        private Locations nowLocation;
        private List<string> locations;
        private TalentItem nowItem;
        private TimeZoneInfo nowTZInfo;

        private void LoadNowTZInfo() => nowTZInfo = TimeZoneInfo.Local;

        public TalentTimerPage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Title = string.Empty;
            }

            locations = new List<string>();
        }

        private void SetToolbar()
        {

        }

        private void LoadLocationList()
        {
            //locations.Clear();
            //locations.AddRange(AppEnvironment.genshinDB.GetAllLocations());
            locations = AppEnvironment.genshinDB.GetAllLocations();
        }

        private void LoadSettings()
        {
            nowServer = (TalentEnv.Servers)Preferences.Get(SettingConstants.ITEM_TALENT_SERVER, 0);
            nowLocation = (Locations)Preferences.Get(SettingConstants.ITEM_TALENT_LOCATION, 0);
        }

        private void CheckNowTalentBook()
        {
            int interval = nowTZInfo.BaseUtcOffset.Hours - TalentEnv.serverUTCs[(int)nowServer];
            int realRenewalHour = TalentEnv.RENEWAL_HOUR + interval;
            var now = DateTime.Now;

            DayOfWeek dowValue = (now.Hour - realRenewalHour) switch
            {
                int result when result < 0 => now.AddDays(-1).DayOfWeek,
                _ => now.DayOfWeek
            };

            nowItem = (from item in AppEnvironment.genshinDB.talentItems
                        where item.Location.Equals(nowLocation) && item.AvailableDayOfWeeks.Contains(dowValue)
                        select item).First();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                LoadSettings();
                LoadNowTZInfo();
                LoadLocationList();
                CheckNowTalentBook();
                RefreshInfo();
            }
            catch (Exception ex)
            {

            }

            SetToolbar();
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
                    var locationDialog = new BaseDialog(AppResources.TalentTimerPage_SelectLocationDialog_Title, new RadioPreferenceView(locations.ToArray(), SettingConstants.ITEM_TALENT_LOCATION));
                    locationDialog.OnClose += delegate
                    {
                        nowLocation = (Locations)Preferences.Get(SettingConstants.ITEM_TALENT_LOCATION, 0);

                        CheckNowTalentBook();
                        RefreshInfo();
                    };

                    await PopupNavigation.Instance.PushAsync(locationDialog);
                    break;
                case "Server":
                    var serverDialog = new BaseDialog(AppResources.TalentTimerPage_SelectServerDialog_Title, new RadioPreferenceView(TalentEnv.serverList, SettingConstants.ITEM_TALENT_SERVER));
                    serverDialog.OnClose += delegate
                    {
                        nowServer = (TalentEnv.Servers)Preferences.Get(SettingConstants.ITEM_TALENT_SERVER, 0);

                        CheckNowTalentBook();
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
                NowServerLabel.Text = $"{AppResources.TalentTimerPage_NowServer_PreLabel} : {TalentEnv.serverList[(int)nowServer]} ({GetUTCString(TalentEnv.serverUTCs[(int)nowServer])})";
                NowRegionUTCLabel.Text = $"{AppResources.TalentTimerPage_NowUTC_PreLabel} : {nowTZInfo.DisplayName} ({GetUTCString(nowTZInfo.BaseUtcOffset.Hours)})";
                NowLocationLabel.Text = $"{AppResources.TalentTimerPage_NowLocation_PreLabel} : {locations[(int)nowLocation]}";
                NowBookPreLabel.Text = nowItem.ItemName.Equals("All") ? AppResources.TalentTimerPage_NowBook_PreLabel_All : AppResources.TalentTimerPage_NowBook_PreLabel;
                NowBookLabel.Text = nowItem.ItemName.Equals("All") ? "" : AppEnvironment.genshinDB.FindLangDic(nowItem.ItemName);
                NowBookImage.Source = ImageSource.FromFile(GetTalentBookImageName());
            }
            catch
            {

            }
        }

        private string GetUTCString(int offset) => $"UTC{((offset >= 0) ? "+" : "")}{offset}";

        private string GetTalentBookImageName()
        {
            return nowItem.ItemName switch
            {
                "Freedom" => "talent_freedom.png",
                "Resistance" => "talent_resistance.png",
                "Ballad" => "talent_ballad.png",
                "Prosperity" => "talent_prosperity.png",
                "Diligence" => "talent_diligence.png",
                "Gold" => "talent_gold.png",
                "All" => $"talent_all_{nowLocation:F}.png",
                _ => ""
            };
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var items = new List<string>();

            if (nowItem.ItemName.Equals("All"))
            {
                items.AddRange(from item in AppEnvironment.genshinDB.talentItems
                               where item.Location.Equals(nowLocation)
                               select item.ItemName);
            }
            else
            {
                items.Add(nowItem.ItemName);
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