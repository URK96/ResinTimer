﻿using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

using AndroidX.Core.Content.Resources;

using Xamarin.Essentials;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Label = "Resin Widget")]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widgetprovider_resin_full")]
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

            ResinEnvironment.LoadValues();
            ResinEnvironment.CalcResin();

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

                remoteViews.SetTextViewText(Resource.Id.ResinWidgetCount, ResinEnvironment.resin.ToString());
                remoteViews.SetTextViewText(Resource.Id.ResinWidgetEndTime, ResinEnvironment.endTime.ToString());

                remoteViews.SetOnClickPendingIntent(Resource.Id.ResinWidgetRootLayout, CreateClickIntent(context, appWidgetIds));

                manager.UpdateAppWidget(id, remoteViews);
            }
        }

        private PendingIntent CreateClickIntent(Context context, int[] appWidgetIds)
        {
            var intent = new Intent(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            return PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
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

            var remoteView = new RemoteViews(context.PackageName, Resource.Layout.ResinWidgetSimple);

            ResinEnvironment.LoadValues();
            ResinEnvironment.CalcResin();

            var intent = new Intent(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            var pIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            remoteView.SetTextViewText(Resource.Id.ResinWidgetSimpleCount, ResinEnvironment.resin.ToString());

            remoteView.SetOnClickPendingIntent(Resource.Id.ResinWidgetSimpleRootLayout, pIntent);

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