using System;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Security;
using System.Net.Cache;
using System.Collections.Generic;

using MinimalHttp.Utilities;
using System.Collections.Specialized;

namespace MinimalHttp
{
    /// <summary>
    /// A callback to validate a HttpCertificate.
    /// </summary>
    /// <param name="certificate">The HttpCertificate to validate.</param>
    /// <returns>A Boolean indicating whether this certificate is valid.</returns>
    public delegate bool CertificateValidation(HttpCertificate certificate);

    /// <summary>
    /// Provides methods to send http requests.
    /// </summary>
    public class HttpClient
    {
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private static readonly int DefaultTimeout = 300 * 1000; // default apache timeout

        private readonly object _lock;

        /// <summary>
        /// Gets or sets a Boolean determining whether the next request should clear the previous Referer.
        /// </summary>
        public bool ClearReferer { get; set; }
        /// <summary>
        /// Gets or sets a Boolean determining whether to disable response caching.
        /// </summary>
        public bool DisableCaching { get; set; }
        /// <summary>
        /// Gets or sets a Boolean determining whether to automatically follow http redirects.
        /// </summary>
        public bool FollowRedirects { get; set; }
        /// <summary>
        /// Gets or sets a Boolean determining whether the http client should be kept alive.
        /// </summary>
        public bool KeepAlive { get; set; }
        /// <summary>
        /// Gets or sets a Boolean determining whether to allow invalid or expired certificates.
        /// </summary>
        public bool AllowInvalidCertificates { get; set; }
        /// <summary>
        /// Gets or sets a Boolean determining whether access to critical properties need to be thread safe.
        /// </summary>
        public bool IsMultiThreaded { get; set; }

        /// <summary>
        /// Gets or sets a value determining the request and response timeout in ms.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets a value determining the host all requests get send to.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Gets or sets a value determining the current location url of this HttpClient.
        /// </summary>
        public string Location { get; private set; }
        /// <summary>
        /// Gets or sets a value determining the user agent string used in http requests.
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// Gets or sets a value determining the referer string used in the next http request.
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// Gets or sets a HttpProxy which can be null or empty if none should be used.
        /// </summary>
        public HttpProxy Proxy { get; set; }
        /// <summary>
        /// Gets or sets a value determining how request and response string data is encoded.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets a List containing all http headers to be send with the next http request.
        /// </summary>
        public WebHeaderCollection Headers { get; private set; }
        /// <summary>
        /// Gets a CookieCollection containing all cookies to be send with the next http request.
        /// </summary>
        public CookieCollection Cookies { get; private set; }

        /// <summary>
        /// Gets a Boolean indicating whether this HttpClient has any HttpHeaders.
        /// </summary>
        public bool HasHeaders => Headers != null && Headers.Count != 0;
        /// <summary>
        /// Gets a Boolean indicating whether this HttpClient has any Cookies.
        /// </summary>
        public bool HasCookies => Cookies != null && Cookies.Count != 0;
        /// <summary>
        /// Gets a Boolean indicating whether this HttpClient is using a HttpProxy.
        /// </summary>
        public bool HasProxy => Proxy != null && Proxy.HasProxy;

        /// <summary>
        /// Gets or sets a CertificateValidation callback method to be used with all following requests.
        /// </summary>
        public CertificateValidation CertificateValidationCallback { get; set; }

        /// <summary>
        /// Initializes a new HttpClient with its default values.
        /// </summary>
        public HttpClient()
        {
            _lock = new object();

            FollowRedirects = true;
            Timeout = DefaultTimeout;
            UserAgent = DefaultUserAgent;

            Proxy = new HttpProxy();

            Encoding = DefaultEncoding;

            Headers = new WebHeaderCollection();

            Cookies = new CookieCollection();
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~HttpClient()
        {
            Proxy = null;
            Encoding = null;
            Headers = null;
            Cookies = null;
        }

        /// <summary>
        /// Clones all properties and initializes a new HttpClient using them.
        /// </summary>
        /// <returns></returns>
        public HttpClient Clone()
        {
            var client = new HttpClient()
            {
                AllowInvalidCertificates = AllowInvalidCertificates,
                CertificateValidationCallback = CertificateValidationCallback,
                ClearReferer = ClearReferer,
                DisableCaching = DisableCaching,
                Encoding = Encoding,
                FollowRedirects = FollowRedirects,
                Host = Host,
                IsMultiThreaded = IsMultiThreaded,
                KeepAlive = KeepAlive,
                Location = Location,
                Referer = Referer,
                Timeout = Timeout,
                UserAgent = UserAgent
            };

            if (HasProxy)
            {
                client.Proxy = new HttpProxy(Proxy.Address, Proxy.Port, Proxy.Username, Proxy.Password);
            }

            if (HasHeaders)
            {
                foreach (var header in EnumerateHeadersSafe())
                {
                    client.Headers.Add(header.Name, header.Value);
                }
            }

            if (HasCookies)
            {
                client.Cookies = CloneCookiesSafe();
            }

            return client;
        }

        /// <summary>
        /// Executes a http GET request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Get(string url) => Send(RequestMethod.Get, url);
        /// <summary>
        /// Executes a http GET request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Get(string url, params HttpParameter[] parameters) => Send(RequestMethod.Get, url + "?" + HttpHelperMethods.GetHttpParameterString(parameters));

        /// <summary>
        /// Executes an async http GET request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> GetAsync(string url) => Task.Factory.StartNew<HttpResponse>(() => Get(url));
        /// <summary>
        /// Executes an async http GET request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> GetAsync(string url, params HttpParameter[] parameters) => Task.Factory.StartNew<HttpResponse>(() => Get(url, parameters));

        /// <summary>
        /// Executes a http HEAD request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Head(string url) => Send(RequestMethod.Head, url);
        /// <summary>
        /// Executes a http HEAD request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Head(string url, params HttpParameter[] parameters) => Send(RequestMethod.Head, url + "?" + HttpHelperMethods.GetHttpParameterString(parameters));

        /// <summary>
        /// Executes an async http HEAD request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> HeadAsync(string url) => Task.Factory.StartNew<HttpResponse>(() => Head(url));
        /// <summary>
        /// Executes an async http HEAD request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> HeadAsync(string url, params HttpParameter[] parameters) => Task.Factory.StartNew<HttpResponse>(() => Head(url, parameters));

        /// <summary>
        /// Executes a http POST requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Post(string url, byte[] data, string contentType = "application/octet-stream") => Send(RequestMethod.Post, url, data, contentType);
        /// <summary>
        /// Executes a http POST requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Post(string url, string data, string contentType = "text/plain")
        {
            if (Encoding == null) Encoding = DefaultEncoding;

            if (string.IsNullOrEmpty(data))
            {
                return Post(url, new byte[0], contentType);
            }
            else
            {
                return Post(url, Encoding.GetBytes(data), contentType);
            }
        }
        /// <summary>
        /// Executes a http POST requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">The http parameters which should be sent with this request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Post(string url, HttpParameter[] parameters, string contentType = "text/plain")
        {
            if (Encoding == null) Encoding = DefaultEncoding;

            if (parameters == null || parameters.Length == 0)
            {
                return Post(url, new byte[0], contentType);
            }
            else
            {
                return Post(url, Encoding.GetBytes(HttpHelperMethods.GetHttpParameterString(parameters)), contentType);
            }
        }

        /// <summary>
        /// Executes an async http POST requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PostAsync(string url, byte[] data, string contentType = "application/octet-stream")
            => Task.Factory.StartNew(() => Post(url, data, contentType));
        /// <summary>
        /// Executes an async http POST requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PostAsync(string url, string data, string contentType = "text/plain")
            => Task.Factory.StartNew(() => Post(url, data, contentType));
        /// <summary>
        /// Executes an async http POST requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">The http parameters which should be sent with this request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PostAsync(string url, HttpParameter[] parameters, string contentType = "text/plain")
            => Task.Factory.StartNew(() => Post(url, parameters, contentType));

        /// <summary>
        /// Executes a http PUT requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Put(string url, byte[] data, string contentType = "application/octet-stream") => Send(RequestMethod.Put, url, data, contentType);
        /// <summary>
        /// Executes a http PUT requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Put(string url, string data, string contentType = "text/plain")
        {
            if (Encoding == null) Encoding = DefaultEncoding;

            if (string.IsNullOrEmpty(data))
            {
                return Put(url, new byte[0], contentType);
            }
            else
            {
                return Put(url, Encoding.GetBytes(data), contentType);
            }
        }
        /// <summary>
        /// Executes a http PUT requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">The http parameters which should be sent with this request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Put(string url, HttpParameter[] parameters, string contentType = "text/plain")
        {
            if (Encoding == null) Encoding = DefaultEncoding;

            if (parameters == null || parameters.Length == 0)
            {
                return Put(url, new byte[0], contentType);
            }
            else
            {
                return Put(url, Encoding.GetBytes(HttpHelperMethods.GetHttpParameterString(parameters)), contentType);
            }
        }

        /// <summary>
        /// Executes an async http PUT requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PutAsync(string url, byte[] data, string contentType = "application/octet-stream")
            => Task.Factory.StartNew(() => Put(url, data, contentType));
        /// <summary>
        /// Executes an async http PUT requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PutAsync(string url, string data, string contentType = "text/plain")
            => Task.Factory.StartNew(() => Put(url, data, contentType));
        /// <summary>
        /// Executes an async http PUT requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">The http parameters which should be sent with this request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PutAsync(string url, HttpParameter[] parameters, string contentType = "text/plain")
            => Task.Factory.StartNew(() => Put(url, parameters, contentType));

        /// <summary>
        /// Executes a http DELETE request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Delete(string url) => Send(RequestMethod.Delete, url);
        /// <summary>
        /// Executes a http DELETE request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Delete(string url, params HttpParameter[] parameters) => Send(RequestMethod.Delete, url + "?" + HttpHelperMethods.GetHttpParameterString(parameters));

        /// <summary>
        /// Executes an async http DELETE request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> DeleteAsync(string url) => Task.Factory.StartNew<HttpResponse>(() => Delete(url));
        /// <summary>
        /// Executes an async http DELETE request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> DeleteAsync(string url, params HttpParameter[] parameters) => Task.Factory.StartNew<HttpResponse>(() => Delete(url, parameters));

        /// <summary>
        /// Executes a http OPTIONS request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Options(string url) => Send(RequestMethod.Options, url);
        /// <summary>
        /// Executes a http OPTIONS request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Options(string url, params HttpParameter[] parameters) => Send(RequestMethod.Options, url + "?" + HttpHelperMethods.GetHttpParameterString(parameters));

        /// <summary>
        /// Executes an async http OPTIONS request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> OptionsAsync(string url) => Task.Factory.StartNew<HttpResponse>(() => Options(url));
        /// <summary>
        /// Executes an async http OPTIONS request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> OptionsAsync(string url, params HttpParameter[] parameters) => Task.Factory.StartNew<HttpResponse>(() => Options(url, parameters));

        /// <summary>
        /// Executes a http TRACE request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Trace(string url) => Send(RequestMethod.Trace, url);
        /// <summary>
        /// Executes a http TRACE request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Trace(string url, params HttpParameter[] parameters) => Send(RequestMethod.Trace, url + "?" + HttpHelperMethods.GetHttpParameterString(parameters));

        /// <summary>
        /// Executes an async http TRACE request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> TraceAsync(string url) => Task.Factory.StartNew<HttpResponse>(() => Trace(url));
        /// <summary>
        /// Executes an async http TRACE request using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">A HttpParameter[] used to append a query string to the URL.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> TraceAsync(string url, params HttpParameter[] parameters) => Task.Factory.StartNew<HttpResponse>(() => Trace(url, parameters));

        /// <summary>
        /// Executes a http PATCH requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Patch(string url, byte[] data, string contentType = "application/octet-stream") => Send(RequestMethod.Put, url, data, contentType);
        /// <summary>
        /// Executes a http PATCH requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Patch(string url, string data, string contentType = "text/plain")
        {
            if (Encoding == null) Encoding = DefaultEncoding;

            if (string.IsNullOrEmpty(data))
            {
                return Patch(url, new byte[0], contentType);
            }
            else
            {
                return Patch(url, Encoding.GetBytes(data), contentType);
            }
        }
        /// <summary>
        /// Executes a http PATCH requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">The http parameters which should be sent with this request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Patch(string url, HttpParameter[] parameters, string contentType = "text/plain")
        {
            if (Encoding == null) Encoding = DefaultEncoding;

            if (parameters == null || parameters.Length == 0)
            {
                return Patch(url, new byte[0], contentType);
            }
            else
            {
                return Patch(url, Encoding.GetBytes(HttpHelperMethods.GetHttpParameterString(parameters)), contentType);
            }
        }

        /// <summary>
        /// Executes an async http PATCH requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PatchAsync(string url, byte[] data, string contentType = "application/octet-stream")
            => Task.Factory.StartNew(() => Patch(url, data, contentType));
        /// <summary>
        /// Executes an async http PATCH requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PatchAsync(string url, string data, string contentType = "text/plain")
            => Task.Factory.StartNew(() => Patch(url, data, contentType));
        /// <summary>
        /// Executes an async http PATCH requests using the given parameters.
        /// </summary>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="parameters">The http parameters which should be sent with this request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> PatchAsync(string url, HttpParameter[] parameters, string contentType = "text/plain")
            => Task.Factory.StartNew(() => Patch(url, parameters, contentType));

        /// <summary>
        /// Sends a http request using the given method and data to the resource url.
        /// </summary>
        /// <param name="method">The RequestMethod to use with this request.</param>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>The HttpResponse returned by the http request.</returns>
        public HttpResponse Send(RequestMethod method, string url, byte[] data = null, string contentType = "application/octet-stream")
        {
            if (method == RequestMethod.Unknown) throw new ArgumentOutOfRangeException(nameof(method));
            if (string.IsNullOrEmpty(nameof(url))) throw new ArgumentNullException(nameof(url));

            if (Encoding == null) Encoding = DefaultEncoding;

            var uri = HttpHelperMethods.CreateUri(url);

            SetLocationSafe(url);

            if (ClearReferer)
            {
                SetRefererSafe(string.Empty);
            }
            
            var request = WebRequest.CreateHttp(uri);

            request.Method = HttpHelperMethods.RequestMethodToString(method);
            request.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
            request.AllowAutoRedirect = FollowRedirects;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.CookieContainer = GetCookiesSafe();
            request.KeepAlive = KeepAlive;
            request.Timeout = Timeout;
            request.Referer = Referer;
            request.UserAgent = UserAgent;
            request.CachePolicy = DisableCaching
                ? new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                : new RequestCachePolicy(RequestCacheLevel.Default);

            if (!string.IsNullOrEmpty(Host))
            {
                request.Host = Host;
            }

            if (HasProxy)
            {
                request.Proxy = Proxy.WebProxy;
                request.Credentials = Proxy.Credentials;
                request.UseDefaultCredentials = !Proxy.HasCredentials;
            }
            else
            {
                request.Proxy = null;
                request.Credentials = null;
                request.UseDefaultCredentials = false;
            }

            if (HasHeaders)
            {
                foreach (var header in EnumerateHeadersSafe())
                {
                    request.Headers.Add(header.Name, header.Value);
                }
            }

            if (string.IsNullOrEmpty(contentType))
            {
                request.ContentType = string.Empty;
            }
            else
            {
                request.ContentType = contentType;
            }

            if (data != null && data.Length != 0)
            {
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            else
            {
                request.ContentLength = 0;
            }

            HttpWebResponse response = null;
            WebException exception = null;
            WebExceptionStatus exceptionStatus = WebExceptionStatus.Success;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                exception = ex;
                exceptionStatus = ex.Status;
                
                if (ex.Response != null)
                {
                    response = (HttpWebResponse)ex.Response;
                }
                else
                {
                    throw;
                }
            }
            
            if (response.ResponseUri == null)
            {
                response.Dispose();
                throw exception;
            }

            if (response.Cookies != null && response.Cookies.Count != 0)
            {
                AddCookiesSafe(response.Cookies);
            }

            SetLocationSafe(response.ResponseUri.OriginalString);

            return request.ServicePoint.Certificate == null
                ? new HttpResponse(response, exceptionStatus)
                : new HttpResponse(response, exceptionStatus, new HttpCertificate(request.ServicePoint.Certificate));
        }
        /// <summary>
        /// Sends an async http request using the given method and data to the resource url.
        /// </summary>
        /// <param name="method">The RequestMethod to use with this request.</param>
        /// <param name="url">The URL string of the web resource.</param>
        /// <param name="data">The data to be send with the request.</param>
        /// <param name="contentType">The mime type of the data sent with the request.</param>
        /// <returns>A Task executing the http request.</returns>
        public Task<HttpResponse> SendAsync(RequestMethod method, string url, byte[] data = null, string contentType = "application/octet-stream")
            => Task.Factory.StartNew<HttpResponse>(() => Send(method, url, data, contentType));

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            if (AllowInvalidCertificates) return true;
            if (policyErrors == SslPolicyErrors.None || policyErrors == SslPolicyErrors.RemoteCertificateNotAvailable) return true;

            var callback = CertificateValidationCallback;

            HttpCertificate httpCertificate = new HttpCertificate(certificate);

            if (callback == null)
            {
                return !httpCertificate.IsExpired && httpCertificate.Verify();
            }
            else
            {
                return callback(httpCertificate);
            }
        }

        private void SetRefererSafe(string value)
        {
            if (IsMultiThreaded)
            {
                lock (_lock)
                {
                    Referer = value;
                }
            }
            else
            {
                Referer = value;
            }
        }

        private void SetLocationSafe(string value)
        {
            if (IsMultiThreaded)
            {
                lock (_lock)
                {
                    Location = value;
                }
            }
            else
            {
                Location = value;
            }
        }

        private void AddCookiesSafe(CookieCollection cookies)
        {
            if (IsMultiThreaded)
            {
                lock (_lock)
                {
                    foreach (Cookie cookie in cookies)
                    {
                        try
                        {
                            Cookies.Add(cookie);
                        }
                        catch { }
                    }
                }
            }
            else
            {
                foreach (Cookie cookie in cookies)
                {
                    try
                    {
                        Cookies.Add(cookie);
                    }
                    catch { }
                }
            }
        }

        private CookieContainer GetCookiesSafe()
        {
            CookieContainer container = new CookieContainer();

            if (IsMultiThreaded)
            {
                lock (_lock)
                {
                    foreach (Cookie cookie in Cookies)
                    {
                        try
                        {
                            container.Add(cookie);
                        }
                        catch { }
                    }
                }
            }
            else
            {
                foreach (Cookie cookie in Cookies)
                {
                    try
                    {
                        container.Add(cookie);
                    }
                    catch { }
                }
            }

            return container;
        }

        private CookieCollection CloneCookiesSafe()
        {
            CookieCollection collection = new CookieCollection();

            if (IsMultiThreaded)
            {
                lock (_lock)
                {
                    foreach (Cookie cookie in Cookies)
                    {
                        collection.Add(cookie);
                    }
                }
            }
            else
            {
                foreach (Cookie cookie in Cookies)
                {
                    collection.Add(cookie);
                }
            }

            return collection;
        }
        
        private IEnumerable<HttpHeader> EnumerateHeadersSafe()
        {
            if (IsMultiThreaded)
            {
                lock (_lock)
                {
                    for (int i = 0; i < Headers.Count; i++)
                    {
                        yield return new HttpHeader(Headers.GetKey(i), Headers.Get(i));
                    }
                }
            }
            else
            {
                for (int i = 0; i < Headers.Count; i++)
                {
                    yield return new HttpHeader(Headers.GetKey(i), Headers.Get(i));
                }
            }
        }
    }
}
