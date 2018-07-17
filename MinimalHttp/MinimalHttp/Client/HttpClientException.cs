using System;
using System.Runtime.Serialization;

namespace MinimalHttp.Client
{
    public class HttpClientException : Exception
    {
        public HttpClientException()
        {
        }

        public HttpClientException(string message) : base(message)
        {
        }

        public HttpClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
