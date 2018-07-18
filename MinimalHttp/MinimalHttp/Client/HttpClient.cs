using System;
using System.Text;
using System.Net;
using System.Net.Cache;

namespace MinimalHttp.Client
{
    public class HttpClient
    {
        public static string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";

        public static readonly string MozillaUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";
        public static readonly string ChromeUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";

        private CookieContainer _cookieContainer;

        public HttpProxy Proxy { get; set; }

        public string UserAgent { get; set; }

        public bool ClearReferer { get; set; }
        public bool AllowAutoRedirect { get; set; }
        public bool KeepAlive { get; set; }

        public string Location { get; private set; }
        public string Referer { get; private set; }

        public Encoding Encoding { get; set; }

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

        ~HttpClient()
        {
            Proxy = null;
            _cookieContainer = null;
        }
        
        public HttpResponse Get(string url)
        {
            return Send(HttpRequestMethod.Get, url);
        }

        public HttpResponse Get(string url, string data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.Length == 0) return Get(url);

            if (!data.StartsWith("?") && !url.EndsWith("?")) data = "?" + data;

            return Send(HttpRequestMethod.Get, url + data);
        }

        public HttpResponse Get(string url, params string[] parameters)
        {
            if (!url.EndsWith("?")) url += "?";

            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            if (parameters.Length == 0) return Get(url);

            for(int i = 0; i < parameters.Length - 1; i++)
            {
                url += parameters[i] + "=" + parameters[i + 1];
            }

            return Send(HttpRequestMethod.Get, url);
        }

        public HttpResponse Get(string url, params HttpParameter[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            if (parameters.Length == 0) return Get(url);

            if (!url.EndsWith("?")) url += "?";

            foreach (var param in parameters)
                url += param.ToString();

            return Send(HttpRequestMethod.Get, url);
        }

        public HttpResponse Post(string url, string content_type, string data)
        {
            if (Encoding == null) Encoding = Encoding.UTF8;

            if (data == null) throw new ArgumentNullException(nameof(data));

            return Send(HttpRequestMethod.Post, url, content_type, Encoding.GetBytes(data));
        }

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
                builder.Append(parameters[i].ToString() + "&");

            builder.Append(parameters[parameters.Length - 1]);

            return Post(url, content_type, builder.ToString());
        }

        public HttpResponse Post(string url, string content_type, byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            return Send(HttpRequestMethod.Post, url, content_type, data);
        }

        public HttpResponse Head(string url)
        {
            return Send(HttpRequestMethod.Head, url);
        }

        private HttpResponse Send(HttpRequestMethod method, string url)
        {
            if (Encoding == null) Encoding = Encoding.UTF8;

            return Send(method, url, null, null);
        }

        private HttpResponse Send(HttpRequestMethod method, string url, string content_type, byte[] data)
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

            if (response == null) throw new NullReferenceException("The http response was null!");

            Location = response.ResponseUri.OriginalString;

            return new HttpResponse(response);
        }
    }
}
