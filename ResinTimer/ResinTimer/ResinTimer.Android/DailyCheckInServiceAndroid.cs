﻿using Android.App;
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
        private const string DailyCheckInZenlessZoneZeroWorkName = "DailyCheckInZenlessZoneZeroWork";

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

        public bool IsRegisteredZenlessZoneZero()
        {
            try
            {
                var datas = WorkManagerInstance.GetWorkInfosForUniqueWork(DailyCheckInZenlessZoneZeroWorkName)
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

        public void Register() =>
            RegisterDailyCheckInPeriodicWorker<DailyCheckInWorker>(DailyCheckInWorkName);

        public void RegisterHonkai() =>
            RegisterDailyCheckInPeriodicWorker<DailyCheckInHonkaiWorker>(DailyCheckInHonkaiWorkName);

        public void RegisterHonkaiStarRail() =>
            RegisterDailyCheckInPeriodicWorker<DailyCheckInHonkaiStarRailWorker>(DailyCheckInHonkaiStarRailWorkName);

        public void RegisterZenlessZoneZero() =>
            RegisterDailyCheckInPeriodicWorker<DailyCheckInZenlessZoneZeroWorker>(DailyCheckInZenlessZoneZeroWorkName);

        private void RegisterDailyCheckInPeriodicWorker<T>(string workName)
            where T : Worker
        {
            Constraints workConstraints = new Constraints.Builder()
                .SetRequiredNetworkType(NetworkType.Connected)
                .Build();

            PeriodicWorkRequest workRequest = PeriodicWorkRequest.Builder
               .From<T>(TimeSpan.FromHours(1), TimeSpan.FromMinutes(5))
               .SetConstraints(workConstraints)
               .Build();

            WorkManagerInstance.EnqueueUniquePeriodicWork(
                workName,
                ExistingPeriodicWorkPolicy.CancelAndReenqueue,
                workRequest);
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

        public void UnregisterZenlessZoneZero()
        {
            WorkManagerInstance.CancelUniqueWork(DailyCheckInZenlessZoneZeroWorkName);
        }
    }
}