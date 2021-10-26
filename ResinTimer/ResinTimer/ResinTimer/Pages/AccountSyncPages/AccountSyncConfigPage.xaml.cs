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
        }

        private void SyncConfigOnChanged(object sender, ToggledEventArgs e)
        {
            SwitchCell cell = sender as SwitchCell;

            string key = cell switch
            {
                _ when cell.Text.Equals(SyncConfigResin.Text)  => SettingConstants.APP_ACCOUNTSYNC_RESIN_ENABLED,
                _ => null
            };

            if (key is not null)
            {
                Preferences.Set(key, e.Value);
            }
        }
    }
}