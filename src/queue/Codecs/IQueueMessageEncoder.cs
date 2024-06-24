namespace Postman.Codecs
{
    /// <summary>
    /// Interface for classes that are responsible for encoding queue message objects into byte streams or arrays.
    /// </summary>
    public interface IQueueMessageEncoder
    {
        Task<byte[]> Encode(IQueueMessage message);

        Task Encode(IQueueMessage message, Stream target);

        Task<byte[]> Encode<T>(T message)
            where T : class, IQueueMessage, new();

        Task Encode<T>(T message, Stream target)
            where T : class, IQueueMessage, new();
    }
}
