using System;
using System.Net;

namespace MinimalHttp.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpProxy
    {
        /// <summary>
        /// The web proxy
        /// </summary>
        private WebProxy _webProxy;
        /// <summary>
        /// The credentials
        /// </summary>
        private NetworkCredential _credentials;

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; private set; }
        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; private set; }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; private set; }
        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                return _webProxy == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has credentials.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has credentials; otherwise, <c>false</c>.
        /// </value>
        public bool HasCredentials
        {
            get
            {
                return _credentials != null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProxy"/> class.
        /// </summary>
        public HttpProxy()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProxy"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public HttpProxy(string address)
        {
            Address = address;

            _webProxy = new WebProxy(address);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProxy"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public HttpProxy(string address, int port)
        {
            Address = address;
            Port = port;

            _webProxy = new WebProxy(address, port);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProxy"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public HttpProxy(string address, string username, string password)
        {
            Address = address;

            _credentials = new NetworkCredential(username, password);
            _webProxy = new WebProxy(address);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProxy"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public HttpProxy(string address, int port, string username, string password)
        {
            Address = address;
            Port = port;

            _credentials = new NetworkCredential(username, password);
            _webProxy = new WebProxy(address, port);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HttpProxy"/> class.
        /// </summary>
        ~HttpProxy()
        {
            _webProxy = null;
            _credentials = null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="HttpProxy"/> to <see cref="WebProxy"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator WebProxy(HttpProxy proxy)
        {
            return proxy._webProxy;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="HttpProxy"/> to <see cref="NetworkCredential"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator NetworkCredential(HttpProxy proxy)
        {
            return proxy._credentials;
        }
    }
}
