using Sontiq.Queue.PubSub.Business.Dto;

namespace Sontiq.Queue.PubSub.Business
{
    public interface IManager<T> where T : BaseDto
    {
        Task Process(T dto);
    }
}
