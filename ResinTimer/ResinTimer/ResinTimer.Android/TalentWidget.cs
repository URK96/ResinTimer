﻿using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

using ResinTimer.Resources;

using System.Collections.Generic;
using System.Linq;

using Xamarin.Essentials;

using static GenshinDB_Core.GenshinDB;
using static ResinTimer.Droid.AndroidAppEnvironment;

using TalentEnv = ResinTimer.TalentEnvironment;
using AppEnv = ResinTimer.AppEnvironment;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Label = "Talent Widget")]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate, Intent.ActionMain, ACTION_NEXT, ACTION_PREVIOUS })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widgetprovider_talent_full")]
    public class TalentWidget : AppWidgetProvider
    {
        public const string ACTION_NEXT = "com.urk.resintimer.viewflipper.NEXT";
        public const string ACTION_PREVIOUS = "com.urk.resintimer.viewflipper.PREVIOUS";

        bool isClick = false;

        private readonly int[] locationImageViewIds =
        {
            Resource.Id.TalentWidgetIconMondstadt,
            Resource.Id.TalentWidgetIconLiyue
        };

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
                        var runIntent = new Intent(context, typeof(MainActivity));
                        runIntent.SetFlags(ActivityFlags.NewTask);
                        runIntent.PutExtra(KEY_TALENTITEM_CLICK, VALUE_TALENTITEM_CLICK);
                        runIntent.PutStringArrayListExtra(KEY_TALENTITEM_LIST, intent.GetStringArrayListExtra(KEY_TALENTITEM_LIST));
                        context.StartActivity(runIntent);
                    }
                    break;
                case ACTION_PREVIOUS:
                    var rv = new RemoteViews(context.PackageName, Resource.Layout.TalentWidget);
                    rv.ShowPrevious(Resource.Id.TalentWidgetIconFlipper);
                    AppWidgetManager.GetInstance(context).UpdateAppWidget(intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, 0), rv);
                    break;
                case ACTION_NEXT:
                    var rv2 = new RemoteViews(context.PackageName, Resource.Layout.TalentWidget);
                    rv2.ShowNext(Resource.Id.TalentWidgetIconFlipper);
                    AppWidgetManager.GetInstance(context).UpdateAppWidget(intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, 0), rv2);
                    break;
            }

            base.OnReceive(context, intent);
        }

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            TalentEnv.LoadSettings();
            AppEnv.LoadNowTZInfo();
            
            if (AppEnv.genshinDB == null)
            {
                AppEnv.genshinDB = new GenshinDB_Core.GenshinDB(AppResources.Culture);
            }

            UpdateLayout(context, appWidgetManager, appWidgetIds);

            if (isClick)
            {
                Toast.MakeText(context, AppResources.TalentWidget_UpdateComplete, ToastLength.Short).Show();

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

                remoteViews.SetTextViewText(Resource.Id.TalentWidgetTitle, AppResources.TalentWidget_Title);

                for (int i = 0; i < locationImageViewIds.Length; ++i)
                {
                    var location = (Locations)i;
                    string itemName = TalentEnv.CheckNowTalentBook(location).ItemName;

                    remoteViews.SetImageViewResource(locationImageViewIds[i], GetTalentBookImageId(itemName, location));
                    CreateTalentIconClickIntent(context, remoteViews, locationImageViewIds[i], itemName, location);
                }

                CreateClickIntent(context, appWidgetIds, id, remoteViews);

                manager.UpdateAppWidget(id, remoteViews);
            }
        }

        private void CreateClickIntent(Context context, int[] ids, int id, RemoteViews remoteViews)
        {
            var intent = new Intent(context, typeof(TalentWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            var prevIntent = new Intent(context, typeof(TalentWidget));
            prevIntent.SetAction(ACTION_PREVIOUS);
            prevIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, id);

            var nextIntent = new Intent(context, typeof(TalentWidget));
            nextIntent.SetAction(ACTION_NEXT);
            nextIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, id);

            remoteViews.SetOnClickPendingIntent(Resource.Id.TalentWidgetRootLayout, PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent));
            remoteViews.SetOnClickPendingIntent(Resource.Id.TalentWidgetPreviousButton, PendingIntent.GetBroadcast(context, 1, prevIntent, PendingIntentFlags.UpdateCurrent));
            remoteViews.SetOnClickPendingIntent(Resource.Id.TalentWidgetNextButton, PendingIntent.GetBroadcast(context, 2, nextIntent, PendingIntentFlags.UpdateCurrent));
        }

        private void CreateTalentIconClickIntent(Context context, RemoteViews remoteViews, int id, string itemName, Locations location)
        {
            var runIntent = new Intent(context, typeof(TalentWidget));
            runIntent.SetAction(Intent.ActionMain);
            runIntent.PutExtra(KEY_RUNAPP, VALUE_RUNAPP);
            runIntent.PutStringArrayListExtra(KEY_TALENTITEM_LIST, CreateTalentList(itemName, location));

            remoteViews.SetOnClickPendingIntent(id, PendingIntent.GetBroadcast(context, id, runIntent, PendingIntentFlags.UpdateCurrent));
        }

        private int GetTalentBookImageId(string itemName, Locations location)
        {
            if (itemName.Equals("All"))
            {
                return location switch
                {
                    Locations.Mondstadt => Resource.Drawable.talent_all_Mondstadt,
                    Locations.Liyue => Resource.Drawable.talent_all_Liyue,
                    _ => 0
                };
            }
            else
            {
                return itemName switch
                {
                    "Freedom" => Resource.Drawable.talent_freedom,
                    "Resistance" => Resource.Drawable.talent_resistance,
                    "Ballad" => Resource.Drawable.talent_ballad,
                    "Prosperity" => Resource.Drawable.talent_prosperity,
                    "Diligence" => Resource.Drawable.talent_diligence,
                    "Gold" => Resource.Drawable.talent_gold,
                    _ => 0
                };
            }
        }

        private List<string> CreateTalentList(string itemName, Locations location)
        {
            var items = new List<string>();

            if (itemName.Equals("All"))
            {
                items.AddRange(from item in AppEnv.genshinDB.talentItems
                               where item.Location.Equals(location)
                               select item.ItemName);
            }
            else
            {
                items.Add(itemName);
            }

            return items;
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