using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using Locations = GenshinDB_Core.GenshinDB.Locations;
using WAEnv = ResinTimer.WeaponAscensionEnvironment;
using AppEnv = ResinTimer.AppEnvironment;
using ResinTimer.Dialogs;
using Rg.Plugins.Popup.Services;
using ResinTimer.Resources;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeaponAscensionTimerPage : ContentPage
    {
        public List<WAItem> WAItems { get; set; }

        public WeaponAscensionTimerPage()
        {
            InitializeComponent();

            WAItems = new List<WAItem>();

            BindingContext = this;
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
            catch (Exception ex) 
            { 
            }
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
                    var locationDialog = new BaseDialog(AppResources.TalentTimerPage_SelectLocationDialog_Title, new RadioPreferenceView(AppEnv.locations.ToArray(), SettingConstants.ITEM_TALENT_LOCATION));
                    locationDialog.OnClose += delegate
                    {
                        WAEnv.Location = (Locations)Preferences.Get(SettingConstants.ITEM_TALENT_LOCATION, 0);

                        WAEnv.CheckNowWAItem();
                        RefreshInfo();
                    };

                    await PopupNavigation.Instance.PushAsync(locationDialog);
                    break;
                default:
                    break;
            }
        }

        private async void RefreshInfo()
        {
            try
            {
                NowServerLabel.Text = $"{AppResources.TalentTimerPage_NowServer_PreLabel} : {AppEnv.serverList[(int)AppEnv.Server]} ({AppEnv.GetUTCString(AppEnv.serverUTCs[(int)AppEnv.Server])})";
                NowRegionUTCLabel.Text = $"{AppResources.TalentTimerPage_NowUTC_PreLabel} : {AppEnv.TZInfo.DisplayName} ({AppEnv.GetUTCString(AppEnv.TZInfo.BaseUtcOffset.Hours)})";
                NowLocationLabel.Text = $"{AppResources.TalentTimerPage_NowLocation_PreLabel} : {AppEnv.locations[(int)WAEnv.Location]}";
                NowWAItemPreLabel.Text = WAEnv.Item.ItemName.Equals("All") ? AppResources.TalentTimerPage_NowBook_PreLabel_All : AppResources.TalentTimerPage_NowBook_PreLabel;
                NowWAItemLabel.Text = WAEnv.Item.ItemName.Equals("All") ? "" : AppEnv.genshinDB.FindLangDic(WAEnv.Item.ItemName);

                CreateItems();

                WAItemCarousel.ItemsSource = new List<WAItem>();
                await Task.Delay(100);
                WAItemCarousel.ItemsSource = WAItems;
            }
            catch { }
        }

        private void CreateItems()
        {
            WAItems.Clear();

            if (WAEnv.Item.ItemName.Equals("All"))
            {
                WAItems.Add(new WAItem(WAEnv.Item, 0));
            }
            else
            {
                for (int i = 1; i <= 4; ++i)
                {
                    WAItems.Add(new WAItem(WAEnv.Item, i));
                }
            }
        }
    }

    public class WAItem
    {
        public GenshinDB_Core.Types.WeaponAscensionItem WAInfo { get; private set; }
        public string ItemName { get; private set; }
        public int Step { get; private set; }
        public string IconString => $"{WAEnv.GetWPItemImageName()}_{Step}.png";

        public WAItem(GenshinDB_Core.Types.WeaponAscensionItem item, int step)
        {
            WAInfo = item;
            Step = step;

            ItemName = (Step == 0) ? WAInfo.ItemName : AppEnv.genshinDB.FindLangDic($"{WAInfo}_{Step}");
        }
    }
}