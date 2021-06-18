using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RealmEnv = ResinTimer.RealmEnvironment;
using RCEnv = ResinTimer.RealmCurrencyEnvironment;
using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditRealmCurrencySetting : ContentPage
    {
        public EditRealmCurrencySetting()
        {
            InitializeComponent();
            InitPicker();
        }

        private void InitPicker()
        {
            RealmRankPicker.ItemsSource = new List<string>()
            {
                AppResources.Realm_Rank_1,
                AppResources.Realm_Rank_2,
                AppResources.Realm_Rank_3,
                AppResources.Realm_Rank_4,
                AppResources.Realm_Rank_5,
                AppResources.Realm_Rank_6,
                AppResources.Realm_Rank_7,
                AppResources.Realm_Rank_8,
                AppResources.Realm_Rank_9,
                AppResources.Realm_Rank_10,
            };
            TrustRankPicker.ItemsSource = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            RealmRankPicker.SelectedIndex = (int)RealmEnv.realmRank;
            TrustRankPicker.SelectedIndex = RealmEnv.trustRank - 1;
        }

        private void ApplySetting()
        {
            RealmEnv.realmRank = (RealmEnv.RealmRank)RealmRankPicker.SelectedIndex;
            RealmEnv.trustRank = TrustRankPicker.SelectedIndex + 1;

            RCEnv.CalcRemainTime();
            RCEnv.SaveValue();

            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                var notiManager = new RealmCurrencyNotiManager();
                notiManager.UpdateNotisTime();
            }

            Navigation.PopAsync();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            switch ((sender as ToolbarItem).Priority)
            {
                case 0:  // Apply
                    ApplySetting();
                    break;
            }
        }
    }
}