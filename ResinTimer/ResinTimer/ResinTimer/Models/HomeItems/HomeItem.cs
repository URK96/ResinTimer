namespace ResinTimer.Models.HomeItems
{
    public abstract class HomeItem
    {
        public abstract string StatusMessage { get; }
        public abstract string OptionalMessage { get; }
        public abstract string ImageString { get; }
        public bool OptionalVisible => !string.IsNullOrWhiteSpace(OptionalMessage);
    }
}
