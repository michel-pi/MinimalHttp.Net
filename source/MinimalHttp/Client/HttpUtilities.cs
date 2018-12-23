using System;
using System.Net;
using System.Net.Sockets;

namespace MinimalHttp.Client
{
    /// <summary>
    /// </summary>
    public class HttpUtilities
    {
        public static IPAddress DnsResolve(string urlIpOrHostname)
        {
            if (string.IsNullOrEmpty(urlIpOrHostname)) throw new ArgumentNullException(nameof(urlIpOrHostname));

            if(IPAddress.TryParse(urlIpOrHostname, out IPAddress parsedAddress))
            {
                return parsedAddress;
            }
            
            if (Uri.TryCreate(urlIpOrHostname, UriKind.Absolute, out Uri uri))
            {
                urlIpOrHostname = uri.DnsSafeHost;
            }

            var resolved = Dns.GetHostAddresses(urlIpOrHostname);

            if (resolved == null || resolved.Length == 0) return IPAddress.None;

            foreach(var ip in resolved)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) return ip;
            }

            return resolved[0];
        }

        /// <summary>
        ///     URLs the encode.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string UrlEncode(string text)
        {
            return Uri.EscapeUriString(text);
        }

        /// <summary>
        ///     URLs the decode.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string UrlDecode(string text)
        {
            return Uri.UnescapeDataString(text);
        }

        /// <summary>
        ///     Creates the type of the content.
        /// </summary>
        /// <param name="mime_type">Type of the MIME.</param>
        /// <param name="charset">The charset.</param>
        /// <param name="boundary">The boundary.</param>
        /// <returns></returns>
        public static string CreateContentType(string mime_type, string charset = null, string boundary = null)
        {
            string result = "";
            int concatenations = 0;

            if (!string.IsNullOrEmpty(mime_type))
            {
                result += mime_type;

                concatenations++;
            }

            if (!string.IsNullOrEmpty(charset))
            {
                if (concatenations > 0) result += "; ";

                result += charset;

                concatenations++;
            }

            if (string.IsNullOrEmpty(boundary)) return result;

            if (concatenations > 0) result += "; ";

            result += boundary;

            return result;
        }
    }
}