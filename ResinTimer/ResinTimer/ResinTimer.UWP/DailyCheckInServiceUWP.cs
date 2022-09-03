using ResinTimer.Services;
using ResinTimer.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(DailyCheckInServiceUWP))]

namespace ResinTimer.UWP
{
    // TODO : Implement Daily Check-In service code on iOS
    internal class DailyCheckInServiceUWP : IDailyCheckInService
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
