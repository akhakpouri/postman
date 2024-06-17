namespace Sontiq.Queue.PubSub.Business.Services;

public interface IFooService
{
    void Confirm(int id, string name);
}