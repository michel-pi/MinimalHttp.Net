using System;
using System.Net;

namespace MinimalHttp
{
    /// <summary>
    /// Represents a proxy to be used with HttpClient requests.
    /// </summary>
    public class HttpProxy
    {
        /// <summary>
        /// A Boolean indicating whether this WebProxy is empty.
        /// </summary>
        public bool IsEmpty => WebProxy == null;

        /// <summary>
        /// A Boolean indicating whether this HttpProxy has Credentials.
        /// </summary>
        public bool HasCredentials => Credentials != null;
        /// <summary>
        /// A Boolean indicating whether this HttpProxy has a WebProxy set.
        /// </summary>
        public bool HasProxy => WebProxy != null;

        /// <summary>
        /// Gets the NetworkCredential used to connect to the WebProxy.
        /// </summary>
        public NetworkCredential Credentials { get; private set; }
        /// <summary>
        /// Gets the WebProxy used by this HttpProxy.
        /// </summary>
        public WebProxy WebProxy { get; private set; }

        /// <summary>
        /// Gets the address of the WebProxy.
        /// </summary>
        public string Address { get; private set; }
        /// <summary>
        /// Gets the port of the WebProxy.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets the username used when authenticating with the WebProxy.
        /// </summary>
        public string Username { get; private set; }
        /// <summary>
        /// Gets the password used when authenticating with the WebProxy.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Initializes a new HttpProxy which is empty.
        /// </summary>
        public HttpProxy()
        {
        }

        /// <summary>
        /// Initializes a new HttpProxy using the given address and other optional parameters.
        /// </summary>
        /// <param name="address">A string representing the address of the proxy server.</param>
        /// <param name="port">A int representing the port number of the proxy server.</param>
        /// <param name="username">A string representing the username used to authenticate at the proxy server.</param>
        /// <param name="password">A string representing the password used to authenticate at the proxy server.</param>
        public HttpProxy(string address, int port = -1, string username = null, string password = null)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));
            if (port < -1 || port > short.MaxValue) throw new ArgumentOutOfRangeException(nameof(port));

            Address = address;
            Port = port;

            Username = username;
            Password = password;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                Credentials = new NetworkCredential(username, password);
            }

            if (port < 1)
            {
                WebProxy = new WebProxy(address);
            }
            else
            {
                WebProxy = new WebProxy(address, port);
            }
        }
    }
}
