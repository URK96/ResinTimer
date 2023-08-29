namespace ResinTimer.Droid.Permissions
{
    internal class NotificationPermission : Xamarin.Essentials.Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new (string, bool)[]
        {
        (Android.Manifest.Permission.PostNotifications, true)
        };
    }
}