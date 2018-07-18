using System;
using System.Runtime.Serialization;

namespace MinimalHttp.Client
{
    public class SslValidationException : Exception
    {
        public SslValidationException()
        {
        }

        public SslValidationException(string message) : base(message)
        {
        }

        public SslValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SslValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
