using Postman.PubSub.Business.Dto;

namespace Postman.PubSub.Business
{
    public interface IManager<in T> where T : BaseDto
    {
        Task Process(T dto);
    }
}
