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

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TalentTimerPage : ContentPage
    {
        private Locations nowLocation;
        private List<string> locations;
        private TalentItem nowItem;

        public TalentTimerPage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Title = string.Empty;
            }

            locations = new List<string>();

            nowLocation = (Locations)Preferences.Get(SettingConstants.ITEM_TALENT_LOCATION, 0);

            LoadLocationList();

            CheckNowTalentBook();
        }

        private void SetToolbar()
        {

        }

        private void LoadLocationList()
        {
            locations.Clear();
            locations.AddRange(AppEnvironment.genshinDB.GetAllLocations());
        }

        private void CheckNowTalentBook()
        {
            DayOfWeek dowValue = DateTime.Now.DayOfWeek;

            nowItem = (from item in AppEnvironment.genshinDB.talentItems
                      where item.Location.Equals(nowLocation) && item.AvailableDayOfWeeks.Contains(dowValue)
                      select item).First();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                LoadLocationList();
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
            try
            {
                var item = sender as ToolbarItem;

                switch (item.Text)
                {
                    case "Location":
                        var dialog = new BaseDialog("Select Location", new RadioPreferenceView(locations.ToArray(), SettingConstants.ITEM_TALENT_LOCATION));
                        dialog.OnClose += delegate
                        {
                            nowLocation = (Locations)Preferences.Get(SettingConstants.ITEM_TALENT_LOCATION, 0);

                            CheckNowTalentBook();
                            RefreshInfo();
                        };

                        await PopupNavigation.Instance.PushAsync(dialog);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void RefreshInfo()
        {
            try
            {
                NowLocationLabel.Text = $"{AppResources.TalentTimerPage_NowLocation_PreLabel} : {locations[(int)nowLocation]}";
                TodayBookLabel.Text = nowItem.ItemName;
                TodayBookImage.Source = ImageSource.FromFile(GetTalentBookImageName());
            }
            catch (Exception ex)
            {

            }
        }

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
                _ => ""
            };
        }
    }
}