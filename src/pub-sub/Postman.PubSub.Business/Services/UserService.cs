using Microsoft.Extensions.Logging;

namespace Postman.PubSub.Business.Services
{
    public class UserService(ILogger<UserService> logger) : IUserService
    {
        readonly ILogger _logger = logger;

        public void Confirm(int id, string email, Guid guid)
        {
            _logger.LogInformation($"User Service received the user info. \n id: {id}, email: {email}, guid: {guid}");
        }
    }
}
