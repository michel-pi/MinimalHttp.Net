using System;

namespace MinimalHttp.Client
{
    public class HttpUtilities
    {
        public static string UrlEncode(string text)
        {
            return Uri.EscapeUriString(text);
        }

        public static string UrlDecode(string text)
        {
            return Uri.UnescapeDataString(text);
        }

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

            if (!string.IsNullOrEmpty(boundary))
            {
                if (concenations > 0) result += "; ";

                result += boundary;
            }

            return result;
        }
    }
}
