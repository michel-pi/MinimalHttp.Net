using System;
using System.IO;
using System.Net;

namespace MinimalHttp.Client
{
    public class HttpResponse
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public Uri ResponseUri { get; private set; }
        public string Server { get; private set; }

        public HttpCertificate Certificate { get; private set; }

        public HttpParameter[] Headers { get; private set; }

        public HttpRequestMethod Method { get; private set; }
        public string ProtocolVersion { get; private set; }

        public CookieCollection Cookies { get; private set; }

        public DateTime LastModified { get; private set; }

        public string ContentEncoding { get; private set; }

        public long ContentLength { get; private set; }

        public string ContentType { get; private set; }
        public string CharacterSet { get; private set; }

        public string Body { get; private set; }

        private HttpResponse()
        {
            throw new NotImplementedException();
        }

        public HttpResponse(HttpWebResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            try
            {
                StatusCode = (int)response.StatusCode;
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

            if (Enum.TryParse<HttpRequestMethod>(response.Method, true, out HttpRequestMethod result))
            {
                Method = result;
            }
            else
            {
                Method = HttpRequestMethod.Unknown;
            }

            ProtocolVersion = response.ProtocolVersion.ToString();

            ResponseUri = response.ResponseUri;

            Server = response.Server;

            if (response.Headers == null) return;

            string[] keys = response.Headers.AllKeys;

            Headers = new HttpParameter[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                Headers[i] = new HttpParameter(keys[i], response.Headers[keys[i]]);
            }

            try
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        Body = reader.ReadToEnd();
                    }
                }
            }
            catch
            {

            }

            response.Dispose();
        }

        public HttpResponse(HttpWebResponse response, HttpCertificate cert) : this(response)
        {
            Certificate = cert;
        }
    }
}
