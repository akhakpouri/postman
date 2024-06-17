using Microsoft.Extensions.Logging;

namespace Sontiq.Queue.PubSub.Business.Services
{
    public class UserService : IUserService
    {
        readonly ILogger _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        public void Confirm(int id, string email, Guid guid)
        {
            _logger.LogInformation($"User Service received the user info. \n id: {id}, email: {email}, guid: {guid}");
        }
    }
}
