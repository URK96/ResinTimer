using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;

namespace ResinTimerUWPTray
{
    internal class ResinTimerUWPTrayContext : ApplicationContext
    {
        private AppServiceConnection connection;
        private readonly Form _ = new Form1();

        public ResinTimerUWPTrayContext()
        {
            NotifyIcon notifyIcon = new()
            {
                Icon = Properties.Resource.AppIcon,
                Visible = true,
                ContextMenuStrip = CreateMenuItems()
            };

            notifyIcon.DoubleClick += OpenParentApp;
        }

        private ContextMenuStrip CreateMenuItems()
        {
            // Create Menu Items

            ToolStripMenuItem openAppMenuItem = new("Open Resin Timer");

            openAppMenuItem.Click += OpenParentApp;

            ToolStripMenuItem sendTestMessageMenuItem = new("Send Test Message");

            sendTestMessageMenuItem.Click += async delegate
            {
                await SendToUWP(new()
                {
                    { "content", "Test Message" }
                });
            };

            ToolStripMenuItem exitMenuItem = new("Exit Tray Service");

            exitMenuItem.Click += ExitMenuItem_Click;


            // Add Menu Items

            ContextMenuStrip contextMenuStrip = new();

            contextMenuStrip.Items.Add(openAppMenuItem);
            contextMenuStrip.Items.Add(sendTestMessageMenuItem);
            contextMenuStrip.Items.Add(exitMenuItem);

            return contextMenuStrip;
        }

        private async void ExitMenuItem_Click(object sender, EventArgs e)
        {
            ValueSet message = new()
            {
                { "exit", "" }
            };

            await SendToUWP(message);

            Application.Exit();
        }

        private async void OpenParentApp(object sender, EventArgs e)
        {
            IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync();

            await appListEntries.FirstOrDefault()?.LaunchAsync();
        }

        private async Task SendToUWP(ValueSet message)
        {
            if (connection is null)
            {
                string packageFamilyName = Package.Current.Id.FamilyName;

                connection = new()
                {
                    PackageFamilyName = Package.Current.Id.FamilyName,
                    AppServiceName = "TrayExtensionService"
                };

                connection.ServiceClosed += Connection_ServiceClosed;

                AppServiceConnectionStatus status = await connection.OpenAsync();

                if (status is not AppServiceConnectionStatus.Success)
                {
                    MessageBox.Show($"Status: {status} {packageFamilyName}");

                    return;
                }
            }

            await connection.SendMessageAsync(message);
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            connection.ServiceClosed -= Connection_ServiceClosed;

            connection = null;
        }
    }
}
