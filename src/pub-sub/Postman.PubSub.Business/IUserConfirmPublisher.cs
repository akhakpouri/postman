using Postman.PubSub.Business.Commands;

namespace Postman.PubSub.Business;

public interface IUserConfirmPublisher : IClientPublisher<UserConfirmMessage> { }