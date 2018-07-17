﻿using System;
using System.IO;
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

        public HttpClient()
        {
            InitializeToDefault();
        }

        public HttpClient(HttpProxy proxy)
        {
            InitializeToDefault();

            Proxy = proxy;
        }

        ~HttpClient()
        {
            Proxy = null;
            _cookieContainer = null;
        }
        
        private HttpResponse Send(HttpRequestMethod method, string url)
        {
            if (method == HttpRequestMethod.Unknown) throw new ArgumentException("The HttpRequestMethod " + method.ToString() + " is not supported!");
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            if (url.Contains(@"\")) url = url.Replace(@"\", @"/");

            if(!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                throw new FormatException("Invalid url format: " + url);
            }

            if(string.IsNullOrEmpty(Referer) && !ClearReferer)
            {
                Referer = Location;
                Location = url;
            }
            else if(ClearReferer)
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

            if(Proxy == null || Proxy.IsEmpty)
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

                if(Proxy.HasCredentials)
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

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch(WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            
            if (response == null) throw new NullReferenceException("THe http response was null!");

            Location = response.ResponseUri.OriginalString;

            // TODO: CookieContainer???

            return new HttpResponse(response);
        }

        private void InitializeToDefault()
        {
            _cookieContainer = new CookieContainer();

            Proxy = null;

            UserAgent = DefaultUserAgent;

            ClearReferer = false;
            AllowAutoRedirect = true;
            KeepAlive = true;

            Location = string.Empty;
            Referer = string.Empty;
        }
    }
}