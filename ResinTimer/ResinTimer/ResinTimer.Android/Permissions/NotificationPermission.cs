using Microsoft.Maui.ApplicationModel;
namespace ResinTimer.Droid.Permissions
{
    internal class NotificationPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new (string, bool)[]
        {
        (Android.Manifest.Permission.PostNotifications, true)
        };
    }
}