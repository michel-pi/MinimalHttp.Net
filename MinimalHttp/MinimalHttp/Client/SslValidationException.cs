using System;
using System.Runtime.Serialization;

namespace MinimalHttp.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class SslValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SslValidationException"/> class.
        /// </summary>
        public SslValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SslValidationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SslValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SslValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public SslValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SslValidationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected SslValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
