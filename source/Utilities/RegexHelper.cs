using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MinimalHttp.Utilities
{
    internal class RegexHelper
    {
        private static Regex _httpHeaderPattern = new Regex(@"^(\S+):\s+(.+);?$");

        public static Tuple<string, string> ParseHttpHeader(string header)
        {
            var match = _httpHeaderPattern.Match(header);

            if (match == null || !match.Success)
            {
                return null;
            }
            else
            {
                return new Tuple<string, string>(match.Groups[1].Value, match.Groups[2].Value);
            }
        }
    }
}
