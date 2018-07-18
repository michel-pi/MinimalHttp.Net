using System;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MinimalHttp.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        /// The default user agent
        /// </summary>
        public static string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";

        /// <summary>
        /// The mozilla user agent
        /// </summary>
        public static readonly string MozillaUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";
        /// <summary>
        /// The chrome user agent
        /// </summary>
        public static readonly string ChromeUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";

        private CookieContainer _cookieContainer;

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        public HttpProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [clear referer].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [clear referer]; otherwise, <c>false</c>.
        /// </value>
        public bool ClearReferer { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [allow automatic redirect].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow automatic redirect]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAutoRedirect { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [keep alive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [keep alive]; otherwise, <c>false</c>.
        /// </value>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; private set; }
        /// <summary>
        /// Gets the referer.
        /// </summary>
        /// <value>
        /// The referer.
        /// </value>
        public string Referer { get; private set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <returns></returns>
        public delegate bool CertificateValidation(HttpCertificate certificate);
        /// <summary>
        /// Gets or sets the certificate validation callback.
        /// </summary>
        /// <value>
        /// The certificate validation callback.
        /// </value>
        public CertificateValidation CertificateValidationCallback { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClient"/> class.
        /// </summary>
        public HttpClient()
        {
            _cookieContainer = new CookieContainer();

            Proxy = null;

            UserAgent = DefaultUserAgent;

            ClearReferer = false;
            AllowAutoRedirect = true;
            KeepAlive = true;

            Location = string.Empty;
            Referer = string.Empty;

            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HttpClient"/> class.
        /// </summary>
        ~HttpClient()
        {
            Proxy = null;
            _cookieContainer = null;
        }

        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public HttpResponse Get(string url)
        {
            return Send(HttpRequestMethod.Get, url);
        }

        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        public HttpResponse Get(string url, string data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.Length == 0) return Get(url);

            if (!data.StartsWith("?") && !url.EndsWith("?")) data = "?" + data;

            return Send(HttpRequestMethod.Get, url + data);
        }

        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public HttpResponse Get(string url, params string[] parameters)
        {
            if (!url.EndsWith("?")) url += "?";

            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (parameters.Length % 2 != 0) throw new ArgumentException("This method only supports an even number of parameters!", nameof(parameters));

            if (parameters.Length == 0) return Get(url);

            StringBuilder builder = new StringBuilder(parameters.Length);

            for (int i = 0; i < parameters.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(parameters[i])) continue;
                builder.Append(parameters[i].ToString() + "=" + parameters[i + 1] + "&");
            }

            if (!string.IsNullOrEmpty(parameters[parameters.Length - 2])) builder.Append(parameters[parameters.Length - 2] + "=" + parameters[parameters.Length - 1]);

            return Send(HttpRequestMethod.Get, url + builder.ToString());
        }

        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public HttpResponse Get(string url, params HttpParameter[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            if (parameters.Length == 0) return Get(url);

            if (!url.EndsWith("?")) url += "?";

            StringBuilder builder = new StringBuilder(parameters.Length);

            for (int i = 0; i < parameters.Length - 1; i++)
            {
                if (parameters[i].IsEmpty) continue;
                builder.Append(parameters[i].ToString() + "&");
            }

            if (!parameters[parameters.Length - 1].IsEmpty) builder.Append(parameters[parameters.Length - 1]);

            return Send(HttpRequestMethod.Get, url + builder.ToString());
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="content_type">Type of the content.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        public HttpResponse Post(string url, string content_type, string data)
        {
            if (Encoding == null) Encoding = Encoding.UTF8;

            if (data == null) throw new ArgumentNullException(nameof(data));

            return Send(HttpRequestMethod.Post, url, content_type, Encoding.GetBytes(data));
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="content_type">Type of the content.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public HttpResponse Post(string url, string content_type, params HttpParameter[] parameters)
        {
            if (Encoding == null) Encoding = Encoding.UTF8;

            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            if(parameters.Length == 0)
            {
                return Send(HttpRequestMethod.Post, url, content_type, new byte[0]);
            }

            StringBuilder builder = new StringBuilder(parameters.Length);

            for (int i = 0; i < parameters.Length - 1; i++)
            {
                if (parameters[i].IsEmpty) continue;
                builder.Append(parameters[i].ToString() + "&");
            }
                
            if(!parameters[parameters.Length - 1].IsEmpty) builder.Append(parameters[parameters.Length - 1]);

            return Post(url, content_type, builder.ToString());
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="content_type">Type of the content.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        public HttpResponse Post(string url, string content_type, byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            return Send(HttpRequestMethod.Post, url, content_type, data);
        }

        /// <summary>
        /// Heads the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public HttpResponse Head(string url)
        {
            return Send(HttpRequestMethod.Head, url);
        }

        /// <summary>
        /// Sends the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public HttpResponse Send(HttpRequestMethod method, string url)
        {
            if (Encoding == null) Encoding = Encoding.UTF8;

            return Send(method, url, null, null);
        }

        /// <summary>
        /// Sends the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="content_type">Type of the content.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The HttpRequestMethod " + method.ToString() + " is not supported!</exception>
        /// <exception cref="ArgumentNullException">url</exception>
        /// <exception cref="FormatException">Invalid url format: " + url</exception>
        /// <exception cref="MinimalHttp.Client.SslValidationException">Failed to verify ssl server certificate</exception>
        public HttpResponse Send(HttpRequestMethod method, string url, string content_type, byte[] data)
        {
            if (Encoding == null) Encoding = Encoding.UTF8;

            if (method == HttpRequestMethod.Unknown) throw new ArgumentException("The HttpRequestMethod " + method.ToString() + " is not supported!");
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            if (url.Contains(@"\")) url = url.Replace(@"\", @"/");

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                throw new FormatException("Invalid url format: " + url);
            }

            if (string.IsNullOrEmpty(Referer) && !ClearReferer)
            {
                Referer = Location;
                Location = url;
            }
            else if (ClearReferer)
            {
                Referer = string.Empty;
            }

            HttpWebRequest request = WebRequest.CreateHttp(uri);

            if (CertificateValidationCallback != null)
            {
                request.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                {
                    var callback = CertificateValidationCallback;

                    if (callback == null) return true;

                    return callback.Invoke(new HttpCertificate(certificate));
                };
            }

            request.AllowAutoRedirect = AllowAutoRedirect;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.CookieContainer = _cookieContainer;
            request.KeepAlive = KeepAlive;
            request.Timeout = 300 * 1000; // default apache timeout

            request.Referer = Referer;

            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            if (Proxy == null || Proxy.IsEmpty)
            {
                request.Proxy = null;
                request.Credentials = null;
                request.UseDefaultCredentials = true;
            }
            else
            {
                request.Proxy = (WebProxy)Proxy;
                request.Credentials = null;
                request.UseDefaultCredentials = true;

                if (Proxy.HasCredentials)
                {
                    request.Credentials = (NetworkCredential)Proxy;
                    request.UseDefaultCredentials = false;
                }
            }

            switch (method)
            {
                case HttpRequestMethod.Get:
                    request.Method = "GET";
                    break;
                case HttpRequestMethod.Post:
                    request.Method = "POST";
                    break;
                case HttpRequestMethod.Head:
                    request.Method = "HEAD";
                    break;
                case HttpRequestMethod.Put:
                    request.Method = "PUT";
                    break;
                case HttpRequestMethod.Delete:
                    request.Method = "DELETE";
                    break;
                default:
                    request.Method = "GET";
                    break;
            }

            if (data != null && data.Length != 0)
            {
                if (content_type != null) request.ContentType = content_type;

                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            
            if (response == null) throw new SslValidationException("Failed to verify ssl server certificate");

            Location = response.ResponseUri.OriginalString;

            if (request.ServicePoint != null && request.ServicePoint.Certificate != null)
            {
                return new HttpResponse(response, new HttpCertificate(request.ServicePoint.Certificate));
            }
            else
            {
                return new HttpResponse(response);
            }
        }
    }
}
