namespace Postman.PubSub.Business.Commands
{
    public class UserConfirmMessage : QueueMessage
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;

    }
}
