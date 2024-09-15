using ResinTimer.Services;
using ResinTimer.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(DailyCheckInServiceUWP))]

namespace ResinTimer.UWP
{
    // TODO : Implement Daily Check-In service code on UWP
    internal class DailyCheckInServiceUWP : IDailyCheckInService
    {
        public bool IsRegistered()
        {
            return false;
        }

        public bool IsRegisteredHonkai()
        {
            return false;
        }

        public bool IsRegisteredHonkaiStarRail()
        {
            return false;
        }

        public bool IsRegisteredZenlessZoneZero()
        {
            return false;
        }

        public void Register()
        {
            
        }

        public void RegisterHonkai()
        {

        }

        public void RegisterHonkaiStarRail()
        {
            
        }

        public void RegisterZenlessZoneZero()
        {
            
        }

        public void Unregister()
        {
            
        }

        public void UnregisterHonkai()
        {

        }

        public void UnregisterHonkaiStarRail()
        {
            
        }

        public void UnregisterZenlessZoneZero()
        {
            
        }
    }
}
