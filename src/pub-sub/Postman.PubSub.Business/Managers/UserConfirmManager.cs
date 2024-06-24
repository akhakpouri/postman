using Postman.PubSub.Business.Commands;
using Postman.PubSub.Business.Dto;

namespace Postman.PubSub.Business.Managers
{
    public class UserConfirmManager(IUserConfirmPublisher publisher) : IManager<UserConfirmDto>
    {
        public async Task Process(UserConfirmDto dto)
        {
            //save the data into db
            //send the message
            var message = new UserConfirmMessage { UserId = dto.Id, Email = dto.Email };
            await publisher.Publish(message);
        }
    }
}
