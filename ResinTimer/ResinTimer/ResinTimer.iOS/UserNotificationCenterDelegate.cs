using Foundation;

using ObjCRuntime;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;

using UserNotifications;

namespace ResinTimer.iOS
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public UserNotificationCenterDelegate() { }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            //base.WillPresentNotification(center, notification, completionHandler);

            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}