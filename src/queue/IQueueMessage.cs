namespace Postman
{
    public interface IQueueMessage
    {
        Guid Id { get; }
        DateTimeOffset Timestamp { get; }
    }
}
