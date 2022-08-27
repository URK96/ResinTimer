using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

using ResinTimer.Models.Materials;
using ResinTimer.Resources;

using System.Collections.Generic;

using Xamarin.Essentials;

using static GenshinDB_Core.GenshinDB;
using static ResinTimer.Droid.AndroidAppEnvironment;

using WAEnv = ResinTimer.WeaponAscensionEnvironment;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Label = "Weapon Ascension Widget", Enabled = true, Exported = true)]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate, Intent.ActionMain, ACTION_NEXT, ACTION_PREVIOUS })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widgetprovider_wa_full")]
    public class WAWidget : AppWidgetProvider
    {
        public const string ACTION_NEXT = "com.urk.resintimer.viewflipper.NEXT";
        public const string ACTION_PREVIOUS = "com.urk.resintimer.viewflipper.PREVIOUS";

        bool isClick = false;

        private readonly int[] locationImageViewIds =
        {
            Resource.Id.WAWidgetIconMondstadt,
            Resource.Id.WAWidgetIconLiyue,
            Resource.Id.WAWidgetIconInazuma,
            Resource.Id.WAWidgetIconSumeru
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
                        runIntent.PutExtra(KEY_WAITEM_CLICK, VALUE_WAITEM_CLICK);
                        runIntent.PutStringArrayListExtra(KEY_WAITEM_LIST, intent.GetStringArrayListExtra(KEY_WAITEM_LIST));
                        context.StartActivity(runIntent);
                    }
                    break;
                case ACTION_PREVIOUS:
                    var rv = new RemoteViews(context.PackageName, Resource.Layout.WAWidget);
                    rv.ShowPrevious(Resource.Id.WAWidgetIconFlipper);
                    AppWidgetManager.GetInstance(context).UpdateAppWidget(intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, 0), rv);
                    break;
                case ACTION_NEXT:
                    var rv2 = new RemoteViews(context.PackageName, Resource.Layout.WAWidget);
                    rv2.ShowNext(Resource.Id.WAWidgetIconFlipper);
                    AppWidgetManager.GetInstance(context).UpdateAppWidget(intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, 0), rv2);
                    break;
            }

            base.OnReceive(context, intent);
        }

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            try
            {
                WAEnv.Instance.UpdateNowWAItems();

                UpdateLayout(context, appWidgetManager, appWidgetIds);
            }
            catch
            {
                Toast.MakeText(context, AppResources.WAWidget_UpdateFail, ToastLength.Short).Show();
            }

            if (isClick)
            {
                Toast.MakeText(context, AppResources.WAWidget_UpdateComplete, ToastLength.Short).Show();

                isClick = false;
            }
        }

        private void UpdateLayout(Context context, AppWidgetManager manager, int[] appWidgetIds)
        {
            foreach (int id in appWidgetIds)
            {
                var remoteViews = (Preferences.Get($"{SettingConstants.WIDGET_BACKGROUND}_{id}", "White")) switch
                {
                    "Black" => new RemoteViews(context.PackageName, Resource.Layout.WAWidget),
                    _ => new RemoteViews(context.PackageName, Resource.Layout.WAWidget)
                };

                remoteViews.SetTextViewText(Resource.Id.WAWidgetTitle, AppResources.WAWidget_Title);

                for (int i = 0; i < locationImageViewIds.Length; ++i)
                {
                    var location = (Locations)i;
                    WAListItem item = WAEnv.Instance.Items
                        .Find(x => (x as WAListItem).Item.Location == location) as WAListItem;
                    string itemName = item.Item.ItemName;

                    remoteViews.SetImageViewResource(locationImageViewIds[i], GetWABookImageId(itemName, location));

                    CreateWAIconClickIntent(context, remoteViews, locationImageViewIds[i], itemName, location);
                }

                CreateClickIntent(context, appWidgetIds, id, remoteViews);

                manager.UpdateAppWidget(id, remoteViews);
            }
        }

        private void CreateClickIntent(Context context, int[] ids, int id, RemoteViews remoteViews)
        {
            var intent = new Intent(context, typeof(WAWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
            intent.PutExtra(KEY_CLICKUPDATE, VALUE_CLICKUPDATE);

            var prevIntent = new Intent(context, typeof(WAWidget));
            prevIntent.SetAction(ACTION_PREVIOUS);
            prevIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, id);

            var nextIntent = new Intent(context, typeof(WAWidget));
            nextIntent.SetAction(ACTION_NEXT);
            nextIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, id);

            remoteViews.SetOnClickPendingIntent(Resource.Id.WAWidgetRootLayout, PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable));
            remoteViews.SetOnClickPendingIntent(Resource.Id.WAWidgetPreviousButton, PendingIntent.GetBroadcast(context, 1, prevIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable));
            remoteViews.SetOnClickPendingIntent(Resource.Id.WAWidgetNextButton, PendingIntent.GetBroadcast(context, 2, nextIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable));
        }

        private void CreateWAIconClickIntent(Context context, RemoteViews remoteViews, int id, string itemName, Locations location)
        {
            Intent runIntent = new Intent(context, typeof(WAWidget));
            runIntent.SetAction(Intent.ActionMain);
            runIntent.PutExtra(KEY_RUNAPP, VALUE_RUNAPP);
            runIntent.PutStringArrayListExtra(KEY_WAITEM_LIST, CreateWAList(itemName, location));

            remoteViews.SetOnClickPendingIntent(id, PendingIntent.GetBroadcast(context, id, runIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable));
        }

        private int GetWABookImageId(string itemName, Locations location)
        {
            if (itemName.Equals("All"))
            {
                return location switch
                {
                    Locations.Mondstadt => Resource.Drawable.wa_all_Mondstadt,
                    Locations.Liyue => Resource.Drawable.wa_all_Liyue,
                    Locations.Inazuma => Resource.Drawable.wa_all_Inazuma,
                    Locations.Sumeru => Resource.Drawable.wa_all_Sumeru,
                    _ => 0
                };
            }
            else
            {
                return itemName switch
                {
                    "Decarabian" => Resource.Drawable.wa_decarabian_4,
                    "Boreal Wolf" => Resource.Drawable.wa_boreal_wolf_4,
                    "The Dandelion Gladiator" => Resource.Drawable.wa_the_dandelion_gladiator_4,
                    "Guyun" => Resource.Drawable.wa_guyun_4,
                    "Mist Veiled" => Resource.Drawable.wa_mist_veiled_4,
                    "Aerosiderite" => Resource.Drawable.wa_aerosiderite_4,
                    "Branch of a Distant Sea" => Resource.Drawable.wa_branch_of_a_distant_sea_4,
                    "Narukami" => Resource.Drawable.wa_narukami_4,
                    "Mask" => Resource.Drawable.wa_mask_4,
                    "Talisman of the Forest Dew" => Resource.Drawable.wa_talisman_of_the_forest_dew_4,
                    "Oasis Garden" => Resource.Drawable.wa_oasis_garden_4,
                    "Scorching Might" => Resource.Drawable.wa_scorching_might_4,
                    _ => 0
                };
            }
        }

        private List<string> CreateWAList(string itemName, Locations location)
        {
            List<string> items = new List<string>();

            foreach (WAListItem item in WAEnv.Instance.Items)
            {
                items.Add(item.Item.ItemName);
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