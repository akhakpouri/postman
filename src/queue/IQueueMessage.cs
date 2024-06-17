namespace Sontiq.Queue
{
    public interface IQueueMessage
    {
        Guid Id { get; }
        DateTimeOffset Timestamp { get; }
    }
}
