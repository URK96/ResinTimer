﻿using ResinTimer.iOS;
using ResinTimer.Services;

[assembly: Xamarin.Forms.Dependency(typeof(DailyCheckInServiceiOS))]

namespace ResinTimer.iOS
{
    // TODO : Implement Daily Check-In service code on iOS
    internal class DailyCheckInServiceiOS : IDailyCheckInService
    {
        public bool IsRegistered()
        {
            return false;
        }

        public void Register()
        {
            
        }

        public void Unregister()
        {
            
        }
    }
}