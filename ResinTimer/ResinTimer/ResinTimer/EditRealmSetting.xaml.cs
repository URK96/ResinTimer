using ResinTimer.Resources;

using System;
using System.Collections.Generic;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RCEnv = ResinTimer.RealmCurrencyEnvironment;
using RFEnv = ResinTimer.RealmFriendshipEnvironment;
using RealmEnv = ResinTimer.RealmEnvironment;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditRealmSetting : ContentPage
    {
        RealmEnv.RealmType type;

        public EditRealmSetting(RealmEnv.RealmType type)
        {
            InitializeComponent();
            InitPicker();

            this.type = type;
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

            switch (type)
            {
                case RealmEnv.RealmType.Currency:
                    RCEnv.CalcRemainTime();
                    RCEnv.SaveValue();
                    break;
                case RealmEnv.RealmType.Friendship:
                    RFEnv.CalcRemainTime();
                    RFEnv.SaveValue();
                    break;
            }

            UpdateNotis();

            Navigation.PopAsync();
        }

        private void UpdateNotis()
        {
            if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
            {
                NotiManager notiManager = type switch
                {
                    RealmEnv.RealmType.Currency => new RealmCurrencyNotiManager(),
                    RealmEnv.RealmType.Friendship => new RealmFriendshipNotiManager(),
                    _ => null
                };

                notiManager?.UpdateNotisTime();
            }
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