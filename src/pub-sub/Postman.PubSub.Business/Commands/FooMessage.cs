namespace Postman.PubSub.Business.Commands
{
    public class FooMessage : QueueMessage
    {
        public int FooId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
