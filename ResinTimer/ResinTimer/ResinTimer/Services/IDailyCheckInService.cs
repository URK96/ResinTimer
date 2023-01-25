namespace ResinTimer.Services
{
    public interface IDailyCheckInService
    {
        void Register();
        void RegisterHonkai();
        bool IsRegistered();
        bool IsRegisteredHonkai();
        void Unregister();
        void UnregisterHonkai();
    }
}
