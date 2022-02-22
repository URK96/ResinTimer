using Android.App;
using Android.Content;
using Android.Widget;

using ResinTimer.Resources;

using System;

using Xamarin.Essentials;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new string[] { Intent.ActionBootCompleted, "android.intent.action.QUICKBOOT_POWERON" })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var scheduledNoti = new NotiScheduleAndroid();
            
            try
            {
                if (!Preferences.Get(SettingConstants.NOTI_ENABLED, false))
                {
                    return;
                }

                scheduledNoti.ScheduleAll();
                scheduledNoti.ScheduleCustomNoti(AppResources.AppName, AppResources.BootAlarmRegisterSuccess, 999, DateTime.Now.AddSeconds(5));
            }
            catch (Exception ex)
            {
                // Toast.MakeText(context, ex.ToString(), ToastLength.Long).Show(); 
                //Toast.MakeText(context, context.Resources.GetString(Resource.String.BootAlarmRegisterFail), ToastLength.Short).Show();

                scheduledNoti.ScheduleCustomNoti(AppResources.AppName, AppResources.BootAlarmRegisterFail, 999, DateTime.Now.AddSeconds(5));
            }
        }
    }
}