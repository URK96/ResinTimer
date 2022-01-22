using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages.AccountSyncPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountSyncConfigPage : ContentPage
    {
        public AccountSyncConfigPage()
        {
            InitializeComponent();

            InitSetting();
        }

        private void InitSetting()
        {
            SyncConfigResin.On = Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_RESIN_ENABLED, false);
            SyncConfigResin.OnChanged += SyncConfigOnChanged;
            SyncConfigRealmCurrency.On = 
                Preferences.Get(SettingConstants.APP_ACCOUNTSYNC_REALMCURRENCY_ENABLED, false);
            SyncConfigRealmCurrency.OnChanged += SyncConfigOnChanged;
        }

        private void SyncConfigOnChanged(object sender, ToggledEventArgs e)
        {
            SwitchCell cell = sender as SwitchCell;

            string key = cell.Text switch
            {
                string cellKey when cellKey == SyncConfigResin.Text => 
                    SettingConstants.APP_ACCOUNTSYNC_RESIN_ENABLED,
                string cellKey when cellKey == SyncConfigRealmCurrency.Text => 
                    SettingConstants.APP_ACCOUNTSYNC_REALMCURRENCY_ENABLED,
                _ => null
            };

            if (key is not null)
            {
                Preferences.Set(key, e.Value);
            }
        }
    }
}