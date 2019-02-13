using System;

namespace MinimalHttp
{
    /// <summary>
    /// Defines request methods for the HttpClient class.
    /// </summary>
    public enum RequestMethod
    {
        /// <summary>
        /// An unknown request method
        /// </summary>
        Unknown,
        /// <summary>
        /// A GET request.
        /// </summary>
        Get,
        /// <summary>
        /// A HEAD request.
        /// </summary>
        Head,
        /// <summary>
        /// A POST request.
        /// </summary>
        Post,
        /// <summary>
        /// A PUT request.
        /// </summary>
        Put,
        /// <summary>
        /// A DELETE request.
        /// </summary>
        Delete,
        /// <summary>
        /// A CONNECT request.
        /// </summary>
        Connect,
        /// <summary>
        /// A OPTIONS request.
        /// </summary>
        Options,
        /// <summary>
        /// A TRACE request.
        /// </summary>
        Trace,
        /// <summary>
        /// A PATCH request.
        /// </summary>
        Patch
    }
}
