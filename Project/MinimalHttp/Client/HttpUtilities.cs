using System;

namespace MinimalHttp.Client
{
    /// <summary>
    /// </summary>
    public class HttpUtilities
    {
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
            int concenations = 0;

            if (!string.IsNullOrEmpty(mime_type))
            {
                result += mime_type;

                concenations++;
            }

            if (!string.IsNullOrEmpty(charset))
            {
                if (concenations > 0) result += "; ";

                result += charset;

                concenations++;
            }

            if (string.IsNullOrEmpty(boundary)) return result;

            if (concenations > 0) result += "; ";

            result += boundary;

            return result;
        }
    }
}