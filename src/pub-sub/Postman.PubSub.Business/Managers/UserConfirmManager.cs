using Sontiq.Queue.PubSub.Business.Commands;
using Sontiq.Queue.PubSub.Business.Dto;

namespace Sontiq.Queue.PubSub.Business.Managers
{
    public class UserConfirmManager : IManager<UserConfirmDto>
    {
        readonly IUserConfirmPublisher _publisher;

        public UserConfirmManager(IUserConfirmPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Process(UserConfirmDto dto)
        {
            //save the data into db
            //send the message
            var message = new UserConfirmMessage { UserId = dto.Id, Email = dto.Email };
            await _publisher.Publish(message);
        }
    }
}
