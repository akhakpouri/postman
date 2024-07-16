using Postman;
using Postman.Codecs;
using Postman.Helpers;

namespace Sontiq.Queue.Codecs
{
    /// <summary>
    /// Abstract class that can be used as base for encoder/decoder (codec) classes. 
    /// Contains convenient cross-calls, so at the end you need to just implement only couple of methods. 
    /// </summary>
    public abstract class QueueMessageCodecBase : IQueueMessageEncoder, IQueueMessageDecoder
    {
        public abstract Task Decode(Stream source, IQueueMessageDecoder.StreamDecodeDelegate decodeDelegate);

        public async Task<IQueueMessage> Decode(byte[] data)
        {
            using var ms = new MemoryStream(data);
            return await Decode(ms);
        }

        public async Task<IQueueMessage> Decode(Stream source)
        {
            IQueueMessage? output = null;
            await Decode(source, async (tl, c) =>
            {
                output = (IQueueMessage)await c.Invoke(TypeHelper.Decode(tl.First()));
            });

            return output;
        }

        public async Task Decode(byte[] data, IQueueMessageDecoder.StreamDecodeDelegate decodeDelegate)
        {
            using var ms = new MemoryStream(data);
            await Decode(ms, decodeDelegate);
        }

        public abstract Task Encode(IQueueMessage message, Stream target);

        public virtual async Task<byte[]> Encode(IQueueMessage message)
        {
            using var ms = new MemoryStream();
            await Encode(message, ms);
            return ms.ToArray();
        }

        public virtual async Task<byte[]> Encode<T>(T message)
            where T : class, IQueueMessage, new()
        {
            return await Encode((IQueueMessage)message);
        }

        public virtual async Task Encode<T>(T message, Stream target)
            where T : class, IQueueMessage, new()
        {
            await Encode((IQueueMessage)message, target);
        }
    }
}
