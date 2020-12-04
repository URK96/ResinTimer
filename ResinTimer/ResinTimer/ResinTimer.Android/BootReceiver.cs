using Android.App;
using Android.Content;
using Android.Widget;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

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
                var list = JsonConvert.DeserializeObject<List<Noti>>(Preferences.Get(SettingConstants.NOTI_LIST, string.Empty));
                var title = Application.Context.Resources.GetString(Resource.String.NotiTitle);
                var text = Application.Context.Resources.GetString(Resource.String.NotiText);
                var notifier = new NotifierAndroid();
                var now = DateTime.Now;

                foreach (var item in list)
                {
                    notifier.Cancel(item.NotiId);

                    if (item.NotiTime > now)
                    {
                        notifier.Notify(new Notification
                        {
                            Title = title,
                            Text = $"{item.Resin}{text}",
                            Id = item.NotiId,
                            NotifyTime = item.NotiTime
                        });
                    }
                }

                Toast.MakeText(context, context.Resources.GetString(Resource.String.BootAlarmRegisterSuccess), ToastLength.Short).Show();
            }
            catch (Exception ex)
            {

            }
        }
    }
}