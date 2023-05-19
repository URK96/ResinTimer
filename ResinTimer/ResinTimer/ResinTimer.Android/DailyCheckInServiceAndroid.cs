using Android.App;
using Android.Runtime;

using AndroidX.Work;

using ResinTimer.Droid;
using ResinTimer.Droid.Workers;
using ResinTimer.Services;

using System;

[assembly: Xamarin.Forms.Dependency(typeof(DailyCheckInServiceAndroid))]

namespace ResinTimer.Droid
{
    internal class DailyCheckInServiceAndroid : IDailyCheckInService
    {
        private const string DailyCheckInWorkName = "DailyCheckInWork";
        private const string DailyCheckInHonkaiWorkName = "DailyCheckInHonkaiWork";
        private const string DailyCheckInHonkaiStarRailWorkName = "DailyCheckInHonkaiStarRailWork";

        public WorkManager WorkManagerInstance => WorkManager.GetInstance(Application.Context);

        public bool IsRegistered()
        {
            try
            {
                var datas = WorkManagerInstance.GetWorkInfosForUniqueWork(DailyCheckInWorkName)
                                               .Get()
                                               .JavaCast<JavaList<WorkInfo>>();
                var workInfo = datas.Get(0) as WorkInfo;
                WorkInfo.State state = workInfo?.GetState();

                return (state is not null) &&
                       ((state == WorkInfo.State.Enqueued) || (state == WorkInfo.State.Running));
            }
            catch
            {
                return false;
            }
        }

        public bool IsRegisteredHonkai()
        {
            try
            {
                var datas = WorkManagerInstance.GetWorkInfosForUniqueWork(DailyCheckInHonkaiWorkName)
                                               .Get()
                                               .JavaCast<JavaList<WorkInfo>>();
                var workInfo = datas.Get(0) as WorkInfo;
                WorkInfo.State state = workInfo?.GetState();

                return (state is not null) &&
                       ((state == WorkInfo.State.Enqueued) || (state == WorkInfo.State.Running));
            }
            catch
            {
                return false;
            }
        }

        public bool IsRegisteredHonkaiStarRail()
        {
            try
            {
                var datas = WorkManagerInstance.GetWorkInfosForUniqueWork(DailyCheckInHonkaiStarRailWorkName)
                                               .Get()
                                               .JavaCast<JavaList<WorkInfo>>();
                var workInfo = datas.Get(0) as WorkInfo;
                WorkInfo.State state = workInfo?.GetState();

                return (state is not null) &&
                       ((state == WorkInfo.State.Enqueued) || (state == WorkInfo.State.Running));
            }
            catch
            {
                return false;
            }
        }

        public void Register()
        {
            PeriodicWorkRequest workRequest = PeriodicWorkRequest.Builder
                .From<DailyCheckInWorker>(TimeSpan.FromHours(1), TimeSpan.FromMinutes(5))
                .SetConstraints(new()
                {
                    RequiredNetworkType = NetworkType.Connected
                })
                .Build();

            WorkManagerInstance.EnqueueUniquePeriodicWork(DailyCheckInWorkName, ExistingPeriodicWorkPolicy.Replace, workRequest);
        }

        public void RegisterHonkai()
        {
            PeriodicWorkRequest workRequest = PeriodicWorkRequest.Builder
                .From<DailyCheckInHonkaiWorker>(TimeSpan.FromHours(1), TimeSpan.FromMinutes(5))
                .SetConstraints(new()
                {
                    RequiredNetworkType = NetworkType.Connected
                })
                .Build();

            WorkManagerInstance.EnqueueUniquePeriodicWork(DailyCheckInHonkaiWorkName, ExistingPeriodicWorkPolicy.Replace, workRequest);
        }

        public void RegisterHonkaiStarRail()
        {
            PeriodicWorkRequest workRequest = PeriodicWorkRequest.Builder
               .From<DailyCheckInHonkaiStarRailWorker>(TimeSpan.FromHours(1), TimeSpan.FromMinutes(5))
               .SetConstraints(new()
               {
                   RequiredNetworkType = NetworkType.Connected
               })
               .Build();

            WorkManagerInstance.EnqueueUniquePeriodicWork(DailyCheckInHonkaiStarRailWorkName, ExistingPeriodicWorkPolicy.Replace, workRequest);
        }

        public void Unregister()
        {
            WorkManagerInstance.CancelUniqueWork(DailyCheckInWorkName);
        }

        public void UnregisterHonkai()
        {
            WorkManagerInstance.CancelUniqueWork(DailyCheckInHonkaiWorkName);
        }

        public void UnregisterHonkaiStarRail()
        {
            WorkManagerInstance.CancelUniqueWork(DailyCheckInHonkaiStarRailWorkName);
        }
    }
}