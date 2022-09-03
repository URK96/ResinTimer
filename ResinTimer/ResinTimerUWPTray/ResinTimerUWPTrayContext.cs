using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;

namespace ResinTimerUWPTray
{
    internal class ResinTimerUWPTrayContext : ApplicationContext
    {
        internal static ResinTimerUWPTrayContext Instance;

        internal AppServiceConnection Connection { get; private set; }

        // Do not remove this instance
        private readonly Form _preventExitInstance = new();

        public ResinTimerUWPTrayContext()
        {
            Instance = this;

            NotifyIcon notifyIcon = new()
            {
                Icon = Properties.Resource.AppIcon,
                Visible = true,
                ContextMenuStrip = CreateMenuItems()
            };

            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            notifyIcon.DoubleClick += OpenParentApp;

            _ = CreateConnection();
        }

        private ContextMenuStrip CreateMenuItems()
        {
            // Create Menu Items

            ToolStripMenuItem openAppMenuItem = new("Open Resin Timer");

            openAppMenuItem.Click += OpenParentApp;

            //ToolStripMenuItem sendTestMessageMenuItem = new("Send Test Message");

            //sendTestMessageMenuItem.Click += async delegate
            //{
            //    await SendToUWP(new()
            //    {
            //        { "content", "Test Message" }
            //    });
            //};

            ToolStripMenuItem exitMenuItem = new("Exit Tray Service");

            exitMenuItem.Click += ExitMenuItem_Click;


            // Add Menu Items

            ContextMenuStrip contextMenuStrip = new();

            contextMenuStrip.Items.Add(openAppMenuItem);
            //contextMenuStrip.Items.Add(sendTestMessageMenuItem);
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

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button is MouseButtons.Left)
            {
                if (TrayInfoForm.Instance is null)
                {
                    new TrayInfoForm().Show();
                }
            }
        }

        private async Task CreateConnection()
        {
            string packageFamilyName = Package.Current.Id.FamilyName;

            Connection = new()
            {
                PackageFamilyName = Package.Current.Id.FamilyName,
                AppServiceName = "TrayExtensionService"
            };

            Connection.ServiceClosed += Connection_ServiceClosed;

            AppServiceConnectionStatus status = await Connection.OpenAsync();

            if (status is not AppServiceConnectionStatus.Success)
            {
                MessageBox.Show($"Status: {status} {packageFamilyName}");

                return;
            }
        }

        internal async Task SendToUWP(ValueSet message)
        {
            if (Connection is null)
            {
                await CreateConnection();
            }

            await Connection.SendMessageAsync(message);
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Connection.ServiceClosed -= Connection_ServiceClosed;

            Connection = null;
        }
    }
}
