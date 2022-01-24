using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;

using AndroidX.Core.Content.Resources;

using ResinTimer.Managers.NotiManagers;
using ResinTimer.Models.Notis;

using System;

using Xamarin.Essentials;

using static ResinTimer.Droid.AndroidAppEnvironment;

using REnv = ResinTimer.ResinEnvironment;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Label = "Resin Widget")]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate, Intent.ActionMain })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widgetprovider_resin_full")]
    public class ResinWidget : AppWidgetProvider
    {
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
            }

            base.OnReceive(context, intent);
        }

        public override async void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            if (REnv.IsSyncEnabled)
            {
                try
                {
                    await REnv.SyncServerData();

                    REnv.SaveValue();

                    if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
                    { 
                        new ResinNotiManager().UpdateNotisTime();
                        new NotiScheduleAndroid().Schedule<ResinNoti>();
                    }
                }
                catch (Exception)
                {
                    Toast.MakeText(context, Resources.AppResources.ResinWidget_UpdateFail, ToastLength.Short).Show();
                }
            }
            else
            {
                REnv.LoadValues();
                REnv.CalcResin();
            }

            UpdateLayout(context, appWidgetManager, appWidgetIds);

            if (isClick)
            {
                Toast.MakeText(context, Resources.AppResources.ResinWidget_UpdateComplete, ToastLength.Short).Show();

                isClick = false;
            }
        }

        private void UpdateLayout(Context context, AppWidgetManager manager, int[] appWidgetIds)
        {
            foreach (int id in appWidgetIds)
            {
                var remoteViews = (Preferences.Get($"{SettingConstants.WIDGET_BACKGROUND}_{id}", "White")) switch
                {
                    "Black" => new RemoteViews(context.PackageName, Resource.Layout.ResinWidget_Black),
                    _ => new RemoteViews(context.PackageName, Resource.Layout.ResinWidget)
                };

                remoteViews.SetTextViewText(Resource.Id.ResinWidgetCount, REnv.Resin.ToString());
                remoteViews.SetTextViewText(Resource.Id.ResinWidgetEndTime, Utils.GetTimeString(REnv.EndTime));

                CreateClickIntent(context, appWidgetIds, remoteViews);

                manager.UpdateAppWidget(id, remoteViews);
            }
        }

        private void CreateClickIntent(Context context, int[] ids, RemoteViews remoteViews)
        {
            var intent = new Intent(context, typeof(ResinWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            var runIntent = new Intent(context, typeof(ResinWidget));
            runIntent.SetAction(Intent.ActionMain);
            runIntent.PutExtra(KEY_RUNAPP, VALUE_RUNAPP);

            remoteViews.SetOnClickPendingIntent(Resource.Id.ResinWidgetRootLayout, PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent));
            remoteViews.SetOnClickPendingIntent(Resource.Id.ResinWidgetIcon, PendingIntent.GetBroadcast(context, 0, runIntent, PendingIntentFlags.UpdateCurrent));
        }

        private GradientDrawable CreateBG(Context context, int id)
        {
            var colorString = Preferences.Get($"{SettingConstants.WIDGET_BACKGROUND}_{id}", "#FFFFFF");
            var roundedDrawable = ResourcesCompat.GetDrawable(context.Resources, Resource.Drawable.rounded_background, null) as GradientDrawable;

            roundedDrawable.SetColor(Color.ParseColor(colorString));

            return roundedDrawable;
        }

        private Color CreateColor(int id)
        {
            var colorString = Preferences.Get($"{SettingConstants.WIDGET_BACKGROUND}_{id}", "#FFFFFF");

            return Color.ParseColor(colorString);
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

    [BroadcastReceiver(Label = "Resin Widget Simple")]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widgetprovider_resin_simple")]
    public class ResinWidgetSimple : AppWidgetProvider
    {
        bool isClick = false;

        public override void OnReceive(Context context, Intent intent)
        {
            var value = intent.GetStringExtra(KEY_CLICKUPDATE);

            isClick = value?.Equals(VALUE_CLICKUPDATE) ?? false;

            base.OnReceive(context, intent);
        }

        public override async void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            if (REnv.IsSyncEnabled)
            {
                try
                {
                    await REnv.SyncServerData();

                    REnv.SaveValue();

                    if (Preferences.Get(SettingConstants.NOTI_ENABLED, false))
                    {
                        new ResinNotiManager().UpdateNotisTime();
                        new NotiScheduleAndroid().Schedule<ResinNoti>();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(context, ex.ToString(), ToastLength.Long).Show();
                }
            }
            else
            {
                REnv.LoadValues();
                REnv.CalcResin();
            }

            UpdateLayout(context, appWidgetManager, appWidgetIds);

            if (isClick)
            {
                Toast.MakeText(context, Resources.AppResources.ResinWidget_UpdateComplete, ToastLength.Short).Show();

                isClick = false;
            }
        }

        private void UpdateLayout(Context context, AppWidgetManager manager, int[] appWidgetIds)
        {
            foreach (int id in appWidgetIds)
            {
                //var remoteViews = (Preferences.Get($"{SettingConstants.WIDGET_BACKGROUND}_{id}", "White")) switch
                //{
                //    "Black" => new RemoteViews(context.PackageName, Resource.Layout.ResinWidget_Black),
                //    _ => new RemoteViews(context.PackageName, Resource.Layout.ResinWidget)
                //};

                var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.ResinWidgetSimple);

                remoteViews.SetTextViewText(Resource.Id.ResinWidgetSimpleCount, REnv.Resin.ToString());

                CreateClickIntent(context, appWidgetIds, remoteViews);

                manager.UpdateAppWidget(id, remoteViews);
            }
        }

        private void CreateClickIntent(Context context, int[] ids, RemoteViews remoteViews)
        {
            var intent = new Intent(context, typeof(ResinWidgetSimple));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            var runIntent = new Intent(context, typeof(ResinWidget));
            runIntent.SetAction(Intent.ActionMain);
            runIntent.PutExtra(KEY_RUNAPP, VALUE_RUNAPP);

            remoteViews.SetOnClickPendingIntent(Resource.Id.ResinWidgetSimpleRootLayout, PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent));
            remoteViews.SetOnClickPendingIntent(Resource.Id.ResinWidgetSimpleIcon, PendingIntent.GetBroadcast(context, 0, runIntent, PendingIntentFlags.UpdateCurrent));
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