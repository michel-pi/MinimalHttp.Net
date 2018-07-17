using System;
using System.Net;

namespace MinimalHttp.Client
{
    public class HttpProxy
    {
        private WebProxy _webProxy;
        private NetworkCredential _credentials;

        public string Address { get; private set; }
        public int Port { get; private set; }

        public string Username { get; private set; }
        public string Password { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return _webProxy == null;
            }
        }

        public bool HasCredentials
        {
            get
            {
                return _credentials != null;
            }
        }

        public HttpProxy()
        {

        }

        public HttpProxy(string address)
        {
            Address = address;

            _webProxy = new WebProxy(address);
        }

        public HttpProxy(string address, int port)
        {
            Address = address;
            Port = port;

            _webProxy = new WebProxy(address, port);
        }

        public HttpProxy(string address, string username, string password)
        {
            Address = address;

            _credentials = new NetworkCredential(username, password);
            _webProxy = new WebProxy(address);
        }

        public HttpProxy(string address, int port, string username, string password)
        {
            Address = address;
            Port = port;

            _credentials = new NetworkCredential(username, password);
            _webProxy = new WebProxy(address, port);
        }

        ~HttpProxy()
        {
            _webProxy = null;
            _credentials = null;
        }

        public static explicit operator WebProxy(HttpProxy proxy)
        {
            return proxy._webProxy;
        }

        public static explicit operator NetworkCredential(HttpProxy proxy)
        {
            return proxy._credentials;
        }
    }
}
