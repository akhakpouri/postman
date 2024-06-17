namespace Sontiq.Queue.PubSub.Business.Commands;

public class MxMessage : QueueMessage
{
    public string AccountNumber { get; set; }
}