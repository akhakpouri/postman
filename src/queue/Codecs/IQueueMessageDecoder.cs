namespace Postman.Codecs
{
    /// <summary>
    /// Interface for classes that are responsible for decoding messages from the queue into concrete objects.
    /// Decoding process uses in-the-middle callback to determine, based on some "header" data of the message, types to which this message should be decoded.
    /// </summary>
    public interface IQueueMessageDecoder
    {
        delegate Task<object> MessageDecodeDelegate(Type type);
        delegate Task StreamDecodeDelegate(IEnumerable<string> typeNames, MessageDecodeDelegate decodeCallback);

        Task<IQueueMessage> Decode(byte[] data);
        Task<IQueueMessage> Decode(Stream source);
        Task Decode(byte[] data, StreamDecodeDelegate decodeDelegate);
        Task Decode(Stream source, StreamDecodeDelegate decodeDelegate);
    }
}
