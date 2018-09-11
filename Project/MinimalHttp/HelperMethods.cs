using System;

using MinimalHttp.Client;

namespace MinimalHttp
{
    internal static class HelperMethods
    {
        public static string RequestMethodToString(HttpRequestMethod method)
        {
            switch (method)
            {
                case HttpRequestMethod.Unknown:
                    return "Unknown";
                case HttpRequestMethod.Get:
                    return "Get";
                case HttpRequestMethod.Post:
                    return "Post";
                case HttpRequestMethod.Head:
                    return "Head";
                case HttpRequestMethod.Put:
                    return "Put";
                case HttpRequestMethod.Delete:
                    return "Delete";
                default:
                    return "Unknown";
            }
        }

        public static HttpRequestMethod StringToRequestMethod(string value)
        {
            if (value == null) return HttpRequestMethod.Unknown;

            value = value.ToLower();

            switch(value)
            {
                case "unknown":
                    return HttpRequestMethod.Unknown;
                case "get":
                    return HttpRequestMethod.Get;
                case "post":
                    return HttpRequestMethod.Post;
                case "head":
                    return HttpRequestMethod.Head;
                case "put":
                    return HttpRequestMethod.Put;
                case "delete":
                    return HttpRequestMethod.Delete;
                default:
                    return HttpRequestMethod.Unknown;
            }
        }
    }
}
