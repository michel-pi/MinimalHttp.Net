using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

using MinimalHttp.Utilities;

namespace MinimalHttp
{
    /// <summary>
    /// Represents the response of a http request executed using a HttpClient.
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// Gets A Boolean indicating whether this response contains an ssl certificate.
        /// </summary>
        public bool HasCertificate => Certificate != null;
        /// <summary>
        /// Gets A Boolean indicating whether this response contains any response headers.
        /// </summary>
        public bool HasHeaders => Headers != null && Headers.Count != 0;

        /// <summary>
        /// Gets a Boolean indicating whether this response has thrown an exception.
        /// </summary>
        public bool HasException => Exception != null;

        /// <summary>
        /// Gets A Boolean indicating whether this response is succeed.
        /// </summary>
        public bool IsSuccess => ExceptionStatus == WebExceptionStatus.Success
            && Exception == null
            && ((int)StatusCode > 199 && (int)StatusCode < 400);

        /// <summary>
        /// Gets the exception thrown while executing the request or null.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets the HttpStatusCode returned by the web server.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// Gets a string with a description of the returned status code.
        /// </summary>
        public string StatusDescription { get; private set; }

        /// <summary>
        /// Gets a value indicating the exception type thrown by the http request.
        /// </summary>
        public WebExceptionStatus ExceptionStatus { get; private set; }

        /// <summary>
        /// Gets a string representing the server which answered the request.
        /// </summary>
        public string Server { get; private set; }
        /// <summary>
        /// Gets a Uri containing the final response url.
        /// </summary>
        public Uri ResponseUri { get; private set; }

        /// <summary>
        /// Gets the http protocol version.
        /// </summary>
        public Version ProtocolVersion { get; private set; }

        /// <summary>
        /// Gets the http RequestMethod used by the request.
        /// </summary>
        public RequestMethod RequestMethod { get; private set; }

        /// <summary>
        /// Gets the HttpCertificate used by the request.
        /// </summary>
        public HttpCertificate Certificate { get; private set; }

        /// <summary>
        /// Gets a list of all headers sent in the response.
        /// </summary>
        public List<HttpHeader> Headers { get; private set; }
        /// <summary>
        /// Gets a list of all cookies sent in the response.
        /// </summary>
        public CookieCollection Cookies { get; private set; }
        
        /// <summary>
        /// Gets the DateTime when the requested content was last modified.
        /// </summary>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Gets a string representing the encoding of the returned content.
        /// </summary>
        public string ContentEncoding { get; private set; }
        /// <summary>
        /// Gets a string representing the mime type of the returned content.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets a value indicating the length of the returned payload.
        /// </summary>
        public long ContentLength { get; private set; }

        /// <summary>
        /// Gets a string representing thecharacter set of the returned content.
        /// </summary>
        public string CharacterSet { get; private set; }
        /// <summary>
        /// Gets a string representing the returned content.
        /// </summary>
        public string Body { get; private set; }

        private HttpResponse()
        {
            Exception = null;
            StatusCode = (HttpStatusCode)0;
            StatusDescription = string.Empty;
            ExceptionStatus = WebExceptionStatus.Success;
            Server = string.Empty;
            ResponseUri = null;
            ProtocolVersion = new Version(0, 0, 0, 0);
            RequestMethod = RequestMethod.Unknown;
            Certificate = null;
            Headers = new List<HttpHeader>();
            Cookies = new CookieCollection();
            LastModified = default(DateTime);
            ContentEncoding = string.Empty;
            ContentType = string.Empty;
            ContentLength = 0L;
            CharacterSet = string.Empty;
            Body = string.Empty;
        }

        /// <summary>
        /// Initializes a new HttpResponse using the given Exception
        /// </summary>
        /// <param name="exception">The Exception.</param>
        public HttpResponse(Exception exception) : this()
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            ExceptionStatus = WebExceptionStatus.UnknownError;
        }

        /// <summary>
        /// Initializes a new HttpResponse using the given Exception and status code.
        /// </summary>
        /// <param name="exception">The Exception.</param>
        /// <param name="exceptionStatus">The WebExceptionStatus code.</param>
        public HttpResponse(Exception exception, WebExceptionStatus exceptionStatus) : this()
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            ExceptionStatus = exceptionStatus;
        }

        /// <summary>
        /// Initializes a new HttpResponse using the given HttpWebResponse and an optional certificate.
        /// </summary>
        /// <param name="response">The HttpWebResponse returned by the http request.</param>
        /// <param name="certificate">The HttpCertificate used with the http request.</param>
        public HttpResponse(HttpWebResponse response, HttpCertificate certificate = null) : this()
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;

            ExceptionStatus = WebExceptionStatus.Success;

            Server = response.Server;
            ResponseUri = response.ResponseUri;

            ProtocolVersion = response.ProtocolVersion;

            RequestMethod = HttpHelperMethods.RequestMethodFromString(response.Method);

            Certificate = certificate;
            
            if (response.Headers != null && response.Headers.Count != 0)
            {
                foreach (var key in response.Headers.AllKeys)
                {
                    Headers.Add(new HttpHeader(key, response.Headers[key]));
                }
            }

            if (response.Cookies != null)
            {
                Cookies = response.Cookies;
            }

            LastModified = response.LastModified;

            ContentEncoding = response.ContentEncoding;
            ContentType = response.ContentType;

            ContentLength = response.ContentLength;

            CharacterSet = response.CharacterSet;

            try
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream ?? throw new InvalidOperationException()))
                    {
                        Body = reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                Body = string.Empty;
            }

            response.Dispose();
        }

        /// <summary>
        /// Initializes a new HttpResponse using the given HttpWebResponse, the WebExceptionStatus and optionally a HttpCertificate.
        /// </summary>
        /// <param name="response">The HttpWebResponse returned by the http request.</param>
        /// <param name="exception">The Exception.</param>
        /// <param name="exceptionStatus">The WebExceptionStatus returned by the WebRequest.</param>
        /// <param name="certificate">The HttpCertificate used with the http request.</param>
        public HttpResponse(HttpWebResponse response, Exception exception, WebExceptionStatus exceptionStatus, HttpCertificate certificate = null) : this(response, certificate)
        {
            Exception = exception;
            ExceptionStatus = exceptionStatus;
            Certificate = certificate;
        }
    }
}
