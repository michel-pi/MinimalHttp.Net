using System;
using System.Runtime.Serialization;

namespace MinimalHttp.Client
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    public class SslValidationException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MinimalHttp.Client.SslValidationException" /> class.
        /// </summary>
        public SslValidationException()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MinimalHttp.Client.SslValidationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SslValidationException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MinimalHttp.Client.SslValidationException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (
        ///     <see langword="Nothing" /> in Visual Basic) if no inner exception is specified.
        /// </param>
        public SslValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MinimalHttp.Client.SslValidationException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected SslValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}