using System;
using System.Text;

namespace MinimalHttp.Utilities
{
    /// <summary>
    /// Provides methods often used with http requests and responses.
    /// </summary>
    public static class HttpHelperMethods
    {
        /// <summary>
        /// Creates a new Uri using the given string.
        /// </summary>
        /// <param name="url">A valid absolute url string.</param>
        /// <returns>The Uri this method creates.</returns>
        public static Uri CreateUri(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            if (Uri.TryCreate(url.Replace(@"\", @"/"), UriKind.Absolute, out var result))
            {
                return result;
            }
            else
            {
                throw new FormatException("Invalid url format: " + url);
            }
        }
        
        /// <summary>
        /// Encodes all undefined characters in an url.
        /// </summary>
        /// <param name="text">The url string to encode.</param>
        /// <returns>The string this method creates.</returns>
        public static string UrlEncode(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == string.Empty) return text;

            return Uri.EscapeUriString(text);
        }

        /// <summary>
        /// Decodes all encoded characters in an url.
        /// </summary>
        /// <param name="text">The url string to decode.</param>
        /// <returns>The string this method creates.</returns>
        public static string UrlDecode(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == string.Empty) return text;

            return Uri.UnescapeDataString(text);
        }

        /// <summary>
        /// Encodes all undefined characters in a data string.
        /// </summary>
        /// <param name="text">The data string to encode.</param>
        /// <returns>The string this method creates.</returns>
        public static string DataEncode(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == string.Empty) return text;

            return Uri.EscapeDataString(text);
        }

        /// <summary>
        /// Decodes all encoded characters in a data string.
        /// </summary>
        /// <param name="text">The data string to encode.</param>
        /// <returns>The string this method creates.</returns>
        public static string DataDecode(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == string.Empty) return text;

            return Uri.UnescapeDataString(text);
        }

        /// <summary>
        /// Creates an url query string from the given HttpParameter[].
        /// </summary>
        /// <param name="parameters">An array of HttpParameter.</param>
        /// <returns>The string this method creates.</returns>
        public static string GetHttpParameterString(params HttpParameter[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            StringBuilder sb = new StringBuilder();

            foreach (var parameter in parameters)
            {
                if (parameter == null) continue;

                sb.Append(parameter.ToString() + "&");
            }

            sb.Length--;

            return sb.ToString();
        }

        /// <summary>
        /// Converts a method string to a RequestMethod enum.
        /// </summary>
        /// <param name="method">A string containing the name of the method.</param>
        /// <returns>The RequestMethod enum this method creates.</returns>
        public static RequestMethod RequestMethodFromString(string method)
        {
            if (string.IsNullOrEmpty(method)) throw new ArgumentNullException(nameof(method));

            switch (method.ToUpper())
            {
                case "GET": return RequestMethod.Get;
                case "HEAD": return RequestMethod.Head;
                case "POST": return RequestMethod.Post;
                case "PUT": return RequestMethod.Put;
                case "DELETE": return RequestMethod.Delete;
                case "CONNECT": return RequestMethod.Post;
                case "OPTIONS": return RequestMethod.Post;
                case "TRACE": return RequestMethod.Post;
                case "PATCH": return RequestMethod.Post;
                default: return RequestMethod.Unknown;
            }
        }

        /// <summary>
        /// Converts a RequestMethod enum to a method string.
        /// </summary>
        /// <param name="method">A RequestMethod enum value.</param>
        /// <returns>The string representation of this RequestMethod.</returns>
        public static string RequestMethodToString(RequestMethod method)
        {
            switch (method)
            {
                case RequestMethod.Unknown:
                    return "UNKNOWN";
                case RequestMethod.Get:
                    return "GET";
                case RequestMethod.Head:
                    return "HEAD";
                case RequestMethod.Post:
                    return "POST";
                case RequestMethod.Put:
                    return "PUT";
                case RequestMethod.Delete:
                    return "DELETE";
                case RequestMethod.Connect:
                    return "CONNECT";
                case RequestMethod.Options:
                    return "OPTIONS";
                case RequestMethod.Trace:
                    return "TRACE";
                case RequestMethod.Patch:
                    return "PATCH";
                default:
                    return "UNKNOWN";
            }
        }
    }
}
