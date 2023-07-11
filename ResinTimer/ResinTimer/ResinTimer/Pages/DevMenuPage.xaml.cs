using System;

using ResinTimer.Models.Notis;
using ResinTimer.Services;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevMenuPage : ContentPage
    {
        public DevMenuPage()
        {
            InitializeComponent();
        }

        private void TestExpeditionSyncItemNoti_Tapped(object sender, EventArgs e)
        {
            ExpeditionNoti noti = new(TimeSpan.FromSeconds(5), ExpeditionEnvironment.ExpeditionType.Sync)
            {
                SyncModeImage = "https://act.hoyoverse.com/hk4e/e20200928calculate/item_avatar_side_icon_u7e7we/e0c6c51fd4242cb8a7be96b57b781a60.png"
            };

            INotiScheduleService scheduledService = DependencyService.Get<INotiScheduleService>();

            scheduledService.Cancel<ExpeditionNoti>();
            scheduledService.ScheduleNotiItem<ExpeditionNoti>(noti);
        }
    }
}