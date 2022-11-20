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
        private const string UniqueWorkName = "DailyCheckInWork";

        public WorkManager WorkManagerInstance => WorkManager.GetInstance(Application.Context);

        public bool IsRegistered()
        {
            try
            {
                var datas = WorkManagerInstance.GetWorkInfosForUniqueWork(UniqueWorkName)
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

            WorkManagerInstance.EnqueueUniquePeriodicWork(UniqueWorkName, ExistingPeriodicWorkPolicy.Replace, workRequest);
        }

        public void Unregister()
        {
            WorkManagerInstance.CancelUniqueWork(UniqueWorkName);
        }
    }
}