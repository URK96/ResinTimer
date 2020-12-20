using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;

namespace ResinTimer.UWP
{
    public static class UWPAppEnvironment
    {
        public static ToastNotifier toastNotifier;

        public static readonly string startupPath = Path.Combine(Windows.Storage.UserDataPaths.GetDefault().RoamingAppData, "Microsoft", "Windows", "Start Menu", "Programs", "Startup");
    }
}
