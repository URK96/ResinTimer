namespace ResinTimer.Services
{
    public interface IDailyCheckInService
    {
        void Register();
        bool IsRegistered();
        void Unregister();
    }
}
