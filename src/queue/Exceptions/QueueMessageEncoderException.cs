using System.Runtime.Serialization;

namespace Postman.Exceptions
{
    [Serializable]
    public class QueueMessageEncoderException : Exception
    {
        public QueueMessageEncoderException(string message) : base(message){}

        public QueueMessageEncoderException(string message, Exception innerException) : base(message, innerException){}

        protected QueueMessageEncoderException(SerializationInfo info, StreamingContext context) : base(info, context){}
    }
}
