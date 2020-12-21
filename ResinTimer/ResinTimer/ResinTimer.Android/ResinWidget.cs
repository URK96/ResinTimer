using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Label = "Resin Widget")]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
    public class ResinWidget : AppWidgetProvider
    {
        const string KEY_CLICKUPDATE = "Key_ClickUpdate";
        const string VALUE_CLICKUPDATE = "Value_ClickUpdate";

        bool isClick = false;

        public override void OnReceive(Context context, Intent intent)
        {
            var value = intent.GetStringExtra(KEY_CLICKUPDATE);

            if (!string.IsNullOrWhiteSpace(value))
            {
                isClick = value.Equals(VALUE_CLICKUPDATE);
            }
            else
            {
                isClick = false;
            }
            
            base.OnReceive(context, intent);
        }

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            var remoteView = new RemoteViews(context.PackageName, Resource.Layout.ResinWidget);

            ResinEnvironment.LoadValues();
            ResinEnvironment.CalcResin();

            var intent = new Intent(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            var pIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            remoteView.SetTextViewText(Resource.Id.ResinWidgetCount, ResinEnvironment.resin.ToString());
            remoteView.SetTextViewText(Resource.Id.ResinWidgetEndTime, ResinEnvironment.endTime.ToString());

            remoteView.SetOnClickPendingIntent(Resource.Id.ResinWidgetRootLayout, pIntent);

            foreach (int id in appWidgetIds)
            {
                appWidgetManager.UpdateAppWidget(id, remoteView);
            }

            if (isClick)
            {
                Toast.MakeText(context, Resources.AppResources.ResinWidget_UpdateComplete, ToastLength.Short).Show();

                isClick = false;
            }
        }

        public override void OnEnabled(Context context)
        {
            base.OnEnabled(context);
        }

        public override void OnDisabled(Context context)
        {
            base.OnDisabled(context);
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            base.OnDeleted(context, appWidgetIds);
        }
    }
}