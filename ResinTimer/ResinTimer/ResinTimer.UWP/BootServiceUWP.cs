using ResinTimer.UWP;

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;

[assembly: Xamarin.Forms.Dependency(typeof(BootServiceUWP))]

namespace ResinTimer.UWP
{
    public class BootServiceUWP : IBootService
    {
        const string scriptName = "ResinTimer Bootstrap.bat";
        const char doubleDot = (char)34;
        readonly string scriptPath = Path.Combine(UWPAppEnvironment.startupPath, scriptName);

        public async Task<bool> Register()
        {
            var startupTask = await StartupTask.GetAsync("NotiBootstrap");
            var result = await startupTask.RequestEnableAsync();

            if (!((result == StartupTaskState.Enabled) ||
                (result == StartupTaskState.EnabledByPolicy)))
            {
                return false;
            }

            return true;
        }

        public async Task Unregister()
        {
            var startupTask = await StartupTask.GetAsync("NotiBootstrap");
            startupTask.Disable();
        }
    }
}
