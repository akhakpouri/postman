namespace Sontiq.Queue.PubSub.Business.Services
{
    public interface IUserService
    {
        void Confirm(int id, string email, Guid guid);
    }
}
