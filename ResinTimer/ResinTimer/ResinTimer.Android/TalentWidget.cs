using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

using Xamarin.Essentials;

using static ResinTimer.Droid.AndroidAppEnvironment;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Label = "Talent Widget")]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate, Intent.ActionMain, TalentWidget.ACTION_NEXT, TalentWidget.ACTION_PREVIOUS })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widgetprovider_talent_full")]
    public class TalentWidget : AppWidgetProvider
    {
        public const string ACTION_NEXT = "com.urk.resintimer.viewflipper.NEXT";
        public const string ACTION_PREVIOUS = "com.urk.resintimer.viewflipper.PREVIOUS";

        bool isClick = false;

        public override void OnReceive(Context context, Intent intent)
        {
            switch (intent.Action)
            {
                case AppWidgetManager.ActionAppwidgetUpdate:
                    var updateValue = intent.GetStringExtra(KEY_CLICKUPDATE);
                    isClick = updateValue?.Equals(VALUE_CLICKUPDATE) ?? false;
                    break;
                case Intent.ActionMain:
                    var runValue = intent.GetStringExtra(KEY_RUNAPP);

                    if (runValue?.Equals(VALUE_RUNAPP) ?? false)
                    {
                        var runIntent = new Intent(context, typeof(SplashActivity));
                        runIntent.SetFlags(ActivityFlags.NewTask);
                        context.StartActivity(runIntent);
                    }
                    break;
                case ACTION_PREVIOUS:
                    var rv = new RemoteViews(context.PackageName, Resource.Layout.TalentWidget);
                    rv.ShowPrevious(Resource.Id.TalentWidgetIconFlipper);
                    break;
                case ACTION_NEXT:
                    var rv2 = new RemoteViews(context.PackageName, Resource.Layout.TalentWidget);
                    rv2.ShowNext(Resource.Id.TalentWidgetIconFlipper);
                    break;
            }

            base.OnReceive(context, intent);
        }

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            ResinEnvironment.LoadValues();
            ResinEnvironment.CalcResin();

            UpdateLayout(context, appWidgetManager, appWidgetIds);

            if (isClick)
            {
                Toast.MakeText(context, "Talent Widget Updated", ToastLength.Short).Show();

                isClick = false;
            }
        }

        private void UpdateLayout(Context context, AppWidgetManager manager, int[] appWidgetIds)
        {
            foreach (int id in appWidgetIds)
            {
                var remoteViews = (Preferences.Get($"{SettingConstants.WIDGET_BACKGROUND}_{id}", "White")) switch
                {
                    "Black" => new RemoteViews(context.PackageName, Resource.Layout.TalentWidget),
                    _ => new RemoteViews(context.PackageName, Resource.Layout.TalentWidget)
                };

                remoteViews.SetImageViewResource(Resource.Id.TalentWidgetIconMondstadt, Resource.Drawable.talent_freedom);
                remoteViews.SetImageViewResource(Resource.Id.TalentWidgetIconLiyue, Resource.Drawable.talent_gold);

                CreateClickIntent(context, appWidgetIds, remoteViews);

                manager.UpdateAppWidget(id, remoteViews);
            }
        }

        private void CreateClickIntent(Context context, int[] ids, RemoteViews remoteViews)
        {
            var intent = new Intent(context, typeof(TalentWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            var runIntent = new Intent(context, typeof(TalentWidget));
            runIntent.SetAction(Intent.ActionMain);
            runIntent.PutExtra(KEY_RUNAPP, VALUE_RUNAPP);

            var prevIntent = new Intent(context, typeof(TalentWidget));
            prevIntent.SetAction(ACTION_PREVIOUS);

            var nextIntent = new Intent(context, typeof(TalentWidget));
            nextIntent.SetAction(ACTION_NEXT);

            remoteViews.SetOnClickPendingIntent(Resource.Id.TalentWidgetRootLayout, PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent));
            remoteViews.SetOnClickPendingIntent(Resource.Id.TalentWidgetIconFlipper, PendingIntent.GetBroadcast(context, 0, runIntent, PendingIntentFlags.UpdateCurrent));
            remoteViews.SetOnClickPendingIntent(Resource.Id.TalentWidgetPreviousButton, PendingIntent.GetBroadcast(context, 0, prevIntent, PendingIntentFlags.UpdateCurrent));
            remoteViews.SetOnClickPendingIntent(Resource.Id.TalentWidgetNextButton, PendingIntent.GetBroadcast(context, 0, nextIntent, PendingIntentFlags.UpdateCurrent));
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