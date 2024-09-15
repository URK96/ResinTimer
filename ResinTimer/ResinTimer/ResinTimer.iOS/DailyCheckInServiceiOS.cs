using ResinTimer.iOS;
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