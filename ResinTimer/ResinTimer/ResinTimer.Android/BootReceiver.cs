using Android.App;
using Android.Content;
using Android.Widget;

using System;

using Xamarin.Essentials;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted, "android.intent.action.QUICKBOOT_POWERON" })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (!Preferences.Get(SettingConstants.NOTI_ENABLED, false))
                {
                    return;
                }

                var scheduledNoti = new ScheduledNotiAndroid();
                scheduledNoti.ScheduleAllNoti();

                Toast.MakeText(context, context.Resources.GetString(Resource.String.BootAlarmRegisterSuccess), ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                // Toast.MakeText(context, ex.ToString(), ToastLength.Long).Show(); 
                Toast.MakeText(context, context.Resources.GetString(Resource.String.BootAlarmRegisterFail), ToastLength.Short).Show();
            }
        }
    }
}