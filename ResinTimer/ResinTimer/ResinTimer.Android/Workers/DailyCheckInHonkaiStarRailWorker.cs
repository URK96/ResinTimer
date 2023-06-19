using Android.App;
using Android.Content;
using Android.Util;

using AndroidX.Core.App;
using AndroidX.Work;

using ResinTimer.Resources;

using System;

using static ResinTimer.Helper.DailyRewardHelper;

namespace ResinTimer.Droid.Workers
{
    public class DailyCheckInHonkaiStarRailWorker : Worker
    {
        private const int NotiId = 9002;

        public DailyCheckInHonkaiStarRailWorker(Context context, WorkerParameters parameters) : base(context, parameters)
        {

        }

        public override Result DoWork()
        {
            SignInResult result;

            try
            {
                Log.WriteLine(LogPriority.Info, "ResinTimer", $"Daily CheckIn Honkai Star Rail Work : {DateTime.Now}");

                result = CheckInHonkaiStarRailTodayDailyReward().Result;

                string message = result switch
                {
                    SignInResult.Success => AppResources.DailyReward_CheckIn_Success,
                    SignInResult.AlreadySignIn => AppResources.DailyReward_CheckIn_AlreadySignIn,

                    _ => AppResources.DailyReward_CheckIn_Fail
                };

                message = $"{message} ({AppResources.GameType_HonkaiStarRail})";

                if (result is not SignInResult.AlreadySignIn)
                {
                    Notification builder = new NotificationCompat.Builder(Application.Context, AndroidAppEnvironment.CHANNEL_ID)
                        .SetAutoCancel(true)
                        .SetVisibility((int)NotificationVisibility.Public)
                        .SetContentIntent(PendingIntent.GetActivity(Application.Context, 0,
                                                                    new Intent(Application.Context, typeof(SplashActivity)),
                                                                    PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable))
                        .SetContentTitle(AppResources.Noti_AutoDailyCheckIn_Title)
                        .SetContentText($"{message}")
                        .SetSmallIcon(Resource.Drawable.resintimer_icon)
                        .Build();

                    var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

                    notificationManager.Notify(NotiId, builder);
                }
            }
            catch (Exception ex)
            {
                Log.Error("ResinTimer Work", ex.ToString());

                return Result.InvokeRetry();
            }

            return (result is SignInResult.Fail) ? Result.InvokeRetry() : Result.InvokeSuccess();
        }
    }
}