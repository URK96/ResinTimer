using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.AccountSyncPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerSyncConfigPage : ContentPage
    {
        public TimerSyncConfigPage()
        {
            InitializeComponent();

            InitSetting();
        }

        private void InitSetting()
        {
            SyncConfigDailyMission.On = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_DAILYMISSION_ENABLED, false);
            SyncConfigDailyMission.OnChanged += SyncConfigOnChanged;

            SyncConfigResin.On = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_RESIN_ENABLED, false);
            SyncConfigResin.OnChanged += SyncConfigOnChanged;

            SyncConfigRealmCurrency.On = 
                Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_REALMCURRENCY_ENABLED, false);
            SyncConfigRealmCurrency.OnChanged += SyncConfigOnChanged;

            SyncConfigExpedition.On = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_EXPEDITION_ENABLED, false);
            SyncConfigExpedition.OnChanged += SyncConfigOnChanged;
        }

        private void SyncConfigOnChanged(object sender, ToggledEventArgs e)
        {
            SwitchCell cell = sender as SwitchCell;

            string key = cell.Text switch
            {
                string cellKey when cellKey == SyncConfigDailyMission.Text =>
                    SettingConstants.APP_ACCOUNTSYNC_DAILYMISSION_ENABLED,
                string cellKey when cellKey == SyncConfigResin.Text => 
                    SettingConstants.APP_ACCOUNTSYNC_RESIN_ENABLED,
                string cellKey when cellKey == SyncConfigRealmCurrency.Text => 
                    SettingConstants.APP_ACCOUNTSYNC_REALMCURRENCY_ENABLED,
                string cellKey when cellKey == SyncConfigExpedition.Text =>
                    SettingConstants.APP_ACCOUNTSYNC_EXPEDITION_ENABLED,
                _ => null
            };

            if (key is not null)
            {
                Preferences.Set(key, e.Value);
            }

            if ((cell == SyncConfigExpedition) &&
                !e.Value)
            {
                new ExpeditionNotiManager().RemoveAllSyncNotiItems<ExpeditionNoti>();
            }
        }
    }
}