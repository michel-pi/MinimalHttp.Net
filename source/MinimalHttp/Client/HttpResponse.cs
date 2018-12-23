using System;
using System.IO;
using System.Net;

namespace MinimalHttp.Client
{
    /// <summary>
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        ///     Prevents a default instance of the <see cref="HttpResponse" /> class from being created.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private HttpResponse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpResponse" /> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <exception cref="ArgumentNullException">response</exception>
        /// <exception cref="InvalidOperationException">HttpWebResponse is already disposed!</exception>
        public HttpResponse(HttpWebResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            try
            {
                StatusCode = (int) response.StatusCode;
            }
            catch (ObjectDisposedException)
            {
                throw new InvalidOperationException("HttpWebResponse is already disposed!");
            }

            StatusDescription = response.StatusDescription;

            CharacterSet = response.CharacterSet;

            ContentEncoding = response.ContentEncoding;
            ContentLength = response.ContentLength;
            ContentType = response.ContentType;

            Cookies = response.Cookies;

            LastModified = response.LastModified;

            Method = HelperMethods.StringToRequestMethod(response.Method);

            ProtocolVersion = response.ProtocolVersion.ToString();

            ResponseUri = response.ResponseUri;

            Server = response.Server;

            if (response.Headers == null) return;

            string[] keys = response.Headers.AllKeys;

            Headers = new HttpParameter[keys.Length];

            for (int i = 0; i < keys.Length; i++)
                Headers[i] = new HttpParameter(keys[i], response.Headers[keys[i]]);

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
                // ignored
            }

            response.Dispose();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MinimalHttp.Client.HttpResponse" /> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cert">The cert.</param>
        public HttpResponse(HttpWebResponse response, HttpCertificate cert) : this(response)
        {
            Certificate = cert;
        }

        /// <summary>
        ///     Gets the status code.
        /// </summary>
        /// <value>
        ///     The status code.
        /// </value>
        public int StatusCode { get; }

        /// <summary>
        ///     Gets the status description.
        /// </summary>
        /// <value>
        ///     The status description.
        /// </value>
        public string StatusDescription { get; }

        /// <summary>
        ///     Gets the response URI.
        /// </summary>
        /// <value>
        ///     The response URI.
        /// </value>
        public Uri ResponseUri { get; }

        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <value>
        ///     The server.
        /// </value>
        public string Server { get; }

        /// <summary>
        ///     Gets the certificate.
        /// </summary>
        /// <value>
        ///     The certificate.
        /// </value>
        public HttpCertificate Certificate { get; }

        /// <summary>
        ///     Gets the headers.
        /// </summary>
        /// <value>
        ///     The headers.
        /// </value>
        public HttpParameter[] Headers { get; }

        /// <summary>
        ///     Gets the method.
        /// </summary>
        /// <value>
        ///     The method.
        /// </value>
        public HttpRequestMethod Method { get; }

        /// <summary>
        ///     Gets the protocol version.
        /// </summary>
        /// <value>
        ///     The protocol version.
        /// </value>
        public string ProtocolVersion { get; }

        /// <summary>
        ///     Gets the cookies.
        /// </summary>
        /// <value>
        ///     The cookies.
        /// </value>
        public CookieCollection Cookies { get; }

        /// <summary>
        ///     Gets the last modified.
        /// </summary>
        /// <value>
        ///     The last modified.
        /// </value>
        public DateTime LastModified { get; }

        /// <summary>
        ///     Gets the content encoding.
        /// </summary>
        /// <value>
        ///     The content encoding.
        /// </value>
        public string ContentEncoding { get; }

        /// <summary>
        ///     Gets the length of the content.
        /// </summary>
        /// <value>
        ///     The length of the content.
        /// </value>
        public long ContentLength { get; }

        /// <summary>
        ///     Gets the type of the content.
        /// </summary>
        /// <value>
        ///     The type of the content.
        /// </value>
        public string ContentType { get; }

        /// <summary>
        ///     Gets the character set.
        /// </summary>
        /// <value>
        ///     The character set.
        /// </value>
        public string CharacterSet { get; }

        /// <summary>
        ///     Gets the body.
        /// </summary>
        /// <value>
        ///     The body.
        /// </value>
        public string Body { get; }
    }
}