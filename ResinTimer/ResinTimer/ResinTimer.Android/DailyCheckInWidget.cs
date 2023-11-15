using System;
using System.Threading.Tasks;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

using ResinTimer.Helper;
using ResinTimer.Resources;

using Xamarin.Essentials;

using static ResinTimer.Droid.AndroidAppEnvironment;

namespace ResinTimer.Droid
{
    [BroadcastReceiver(Label = "Daily Check-In Widget", Enabled = true, Exported = true)]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate, Intent.ActionMain, ACTION_CHECKIN })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widgetprovider_dailycheckin_full")]
    public class DailyCheckInWidget : AppWidgetProvider
    {
        private enum GameTypeEnum { Genshin, Honkai3rd, HonkaiStarRail };

        public const string ACTION_CHECKIN = "com.urk.resintimer.button.DailyCheckIn.CHECKIN";

        bool isClick = false;
        private bool isUpdating = false;
        private (bool Genshin, bool Honkai3rd, bool HonkaiStarRail) CheckInResult = (false, false, false);

        public override void OnReceive(Context context, Intent intent)
        {
            switch (intent.Action)
            {
                case AppWidgetManager.ActionAppwidgetUpdate:
                    var updateValue = intent.GetStringExtra(KEY_CLICKUPDATE);
                    isClick = updateValue?.Equals(VALUE_CLICKUPDATE) ?? false;
                    break;
                case Intent.ActionMain:
                    //var runValue = intent.GetStringExtra(KEY_RUNAPP);

                    //if (runValue?.Equals(VALUE_RUNAPP) ?? false)
                    //{
                    //    var runIntent = new Intent(context, typeof(MainActivity));
                    //    runIntent.SetFlags(ActivityFlags.NewTask);
                    //    runIntent.PutExtra(KEY_TALENTITEM_CLICK, VALUE_TALENTITEM_CLICK);
                    //    runIntent.PutStringArrayListExtra(KEY_TALENTITEM_LIST, intent.GetStringArrayListExtra(KEY_TALENTITEM_LIST));
                    //    context.StartActivity(runIntent);
                    //}
                    break;
                case ACTION_CHECKIN:
                    var rv = new RemoteViews(context.PackageName, Resource.Layout.DailyCheckInWidget);
                    //rv.SetDisplayedChild(Resource.Id.TalentWidgetIconFlipper, locationIndex);
                    AppWidgetManager.GetInstance(context).UpdateAppWidget(intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, 0), rv);
                    break;
            }

            base.OnReceive(context, intent);
        }

        public override async void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            isUpdating = true;

            try
            {
                UpdateLayout(context, appWidgetManager, appWidgetIds);

                CheckInResult = 
                    (await RunSignInProcess(GameTypeEnum.Genshin),
                     await RunSignInProcess(GameTypeEnum.Honkai3rd),
                     await RunSignInProcess(GameTypeEnum.HonkaiStarRail));
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, "Fail to run check-in process", ToastLength.Short).Show();

                CheckInResult = (false, false, false);
            }

            try
            {
                isUpdating = false;

                UpdateLayout(context, appWidgetManager, appWidgetIds);

                Toast.MakeText(context, "Success to update check-in status", ToastLength.Short).Show();
            }
            catch
            {
                Toast.MakeText(context, "Fail to update check-in status", ToastLength.Short).Show();
            }


            // Local Functions

            async Task<bool> RunSignInProcess(GameTypeEnum gameType)
            {
                DailyRewardHelper.SignInResult result = gameType switch
                {
                    GameTypeEnum.Genshin => await DailyRewardHelper.CheckInTodayDailyReward(),
                    GameTypeEnum.Honkai3rd => await DailyRewardHelper.CheckInHonkaiTodayDailyReward(),
                    GameTypeEnum.HonkaiStarRail => await DailyRewardHelper.CheckInHonkaiStarRailTodayDailyReward(),

                    _ => await DailyRewardHelper.CheckInTodayDailyReward()
                };

                return result is not DailyRewardHelper.SignInResult.Fail;
            }
        }

        private void UpdateLayout(Context context, AppWidgetManager manager, int[] appWidgetIds)
        {
            foreach (int id in appWidgetIds)
            {
                var remoteViews = Preferences.Get($"{SettingConstants.WIDGET_BACKGROUND}_{id}", "White") switch
                {
                    "Black" => new RemoteViews(context.PackageName, Resource.Layout.DailyCheckInWidget),

                    _ => new RemoteViews(context.PackageName, Resource.Layout.DailyCheckInWidget)
                };

                remoteViews.SetTextViewText(
                    Resource.Id.DailyCheckInWidgetTitle,
                    AppResources.DailyCheckInWidget_Title);
                remoteViews.SetTextViewText(
                    Resource.Id.DailyCheckInWidgetGenshinStatusTitle,
                    AppResources.GameType_Genshin);
                remoteViews.SetTextViewText(
                    Resource.Id.DailyCheckInWidgetHonkai3rdStatusTitle,
                    AppResources.GameType_Honkai3rd);
                remoteViews.SetTextViewText(
                    Resource.Id.DailyCheckInWidgetHonkaiStarRailStatusTitle,
                    AppResources.GameType_HonkaiStarRail_Compat);
                remoteViews.SetTextViewText(
                    Resource.Id.DailyCheckInWidgetCheckInButton,
                    AppResources.DailyCheckInWidget_CheckIn);

                if (isUpdating)
                {
                    int busyIconResourceId = Resource.Drawable.circle_gray_icon;

                    remoteViews.SetImageViewResource(
                        Resource.Id.DailyCheckInWidgetGenshinStatusIcon,
                        busyIconResourceId);
                    remoteViews.SetImageViewResource(
                        Resource.Id.DailyCheckInWidgetHonkai3rdStatusIcon,
                        busyIconResourceId);
                    remoteViews.SetImageViewResource(
                        Resource.Id.DailyCheckInWidgetHonkaiStarRailStatusIcon,
                        busyIconResourceId);
                }
                else
                {
                    int successIconResourceId = Resource.Drawable.circle_green_icon;
                    int failIconResourceId = Resource.Drawable.circle_orange_icon;

                    remoteViews.SetImageViewResource(
                        Resource.Id.DailyCheckInWidgetGenshinStatusIcon,
                        CheckInResult.Genshin ? successIconResourceId : failIconResourceId);
                    remoteViews.SetImageViewResource(
                        Resource.Id.DailyCheckInWidgetHonkai3rdStatusIcon,
                        CheckInResult.Honkai3rd ? successIconResourceId : failIconResourceId);
                    remoteViews.SetImageViewResource(
                        Resource.Id.DailyCheckInWidgetHonkaiStarRailStatusIcon,
                        CheckInResult.HonkaiStarRail ? successIconResourceId : failIconResourceId);
                }

                CreateClickIntent(context, appWidgetIds, id, remoteViews);
                manager.UpdateAppWidget(id, remoteViews);
            }
        }

        private void CreateClickIntent(Context context, int[] ids, int id, RemoteViews remoteViews)
        {
            var updateIntent = new Intent(context, typeof(DailyCheckInWidget))
                .SetAction(AppWidgetManager.ActionAppwidgetUpdate)
                .PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);

            remoteViews.SetOnClickPendingIntent(
                Resource.Id.DailyCheckInWidgetCheckInButton,
                PendingIntent.GetBroadcast(
                    context,
                    0,
                    updateIntent,
                    PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable));
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