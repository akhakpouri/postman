namespace Postman;

public class QueueMessage : IQueueMessage
{
    public Guid Id => Guid.NewGuid();
    public DateTimeOffset Timestamp => DateTimeOffset.Now;
}