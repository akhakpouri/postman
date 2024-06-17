using Sontiq.Queue.Exceptions;
using Sontiq.Queue.Helpers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sontiq.Queue.Codecs
{
    public class StandardQueueMessageCodec : QueueMessageCodecBase
    {
        protected static readonly JsonSerializerOptions OPTIONS = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly TypeHelper typeHelper = new TypeHelper();

        public StandardQueueMessageCodec()
        {
        }

        public StandardQueueMessageCodec(IEnumerable<JsonConverter> jsonConverters)
        {
            if (jsonConverters != null)
            {
                foreach (var converter in jsonConverters)
                {
                    OPTIONS.Converters.Add(converter);
                }
            }
        }

        public override async Task Decode(Stream source, IQueueMessageDecoder.StreamDecodeDelegate decodeDelegate)
        {
            var typesCount = source.ReadByte();
            var messageLengthBytes = new byte[4];

            await source.ReadAsync(messageLengthBytes, 0, 4);
            var messageLength = BitConverter.ToInt32(messageLengthBytes, 0);

            var types = new List<string>();

            for (int i = 0; i < typesCount; i++)
            {
                var len = source.ReadByte();
                var typeBytes = new byte[len];
                await source.ReadAsync(typeBytes, 0, len);

                types.Add(Encoding.UTF8.GetString(typeBytes));
            }

            var doc = new byte[messageLength];
            await source.ReadAsync(doc, 0, messageLength);

            await decodeDelegate.Invoke(types, targetType => Task.FromResult(JsonSerializer.Deserialize(doc, targetType, OPTIONS)));
        }

        public override async Task Encode(IQueueMessage message, Stream target)
        {
            try
            {
                var type = message.GetType();
                var typesBytes = typeHelper.GetQueueMessageTypesFromHierarchy(type).Where(x => !type.Equals(x)).Select(x => Encoding.UTF8.GetBytes(typeHelper.EncodeType(x))).ToList();
                typesBytes.Insert(0, Encoding.UTF8.GetBytes(typeHelper.EncodeType(type)));
                var messageBytes = JsonSerializer.SerializeToUtf8Bytes(message, type, OPTIONS);

                // Write number of type hints
                target.WriteByte((byte)typesBytes.Count);

                // Write length of the message
                await target.WriteAsync(BitConverter.GetBytes(messageBytes.Length));

                // Write all types
                foreach (var bytes in typesBytes)
                {
                    target.WriteByte((byte)bytes.Length);
                    await target.WriteAsync(bytes, 0, bytes.Length);
                }

                await target.WriteAsync(messageBytes, 0, messageBytes.Length);
                await target.FlushAsync();
            }
            catch (Exception e)
            {
                throw new QueueMessageEncoderException($"Exception while encoding queue message {message.Id}.", e);
            }
        }
    }
}
