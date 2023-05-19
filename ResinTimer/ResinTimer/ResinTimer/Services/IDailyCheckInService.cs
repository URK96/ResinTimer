namespace ResinTimer.Services
{
    public interface IDailyCheckInService
    {
        void Register();
        void RegisterHonkai();
        void RegisterHonkaiStarRail();
        bool IsRegistered();
        bool IsRegisteredHonkai();
        bool IsRegisteredHonkaiStarRail();
        void Unregister();
        void UnregisterHonkai();
        void UnregisterHonkaiStarRail();
    }
}
