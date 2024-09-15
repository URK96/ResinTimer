namespace ResinTimer.Services
{
    public interface IDailyCheckInService
    {
        void Register();
        void RegisterHonkai();
        void RegisterHonkaiStarRail();
        void RegisterZenlessZoneZero();
        bool IsRegistered();
        bool IsRegisteredHonkai();
        bool IsRegisteredHonkaiStarRail();
        bool IsRegisteredZenlessZoneZero();
        void Unregister();
        void UnregisterHonkai();
        void UnregisterHonkaiStarRail();
        void UnregisterZenlessZoneZero();
    }
}
