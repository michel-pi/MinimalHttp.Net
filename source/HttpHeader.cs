using System;

using MinimalHttp.Utilities;

namespace MinimalHttp
{
    /// <summary>
    /// Represents a single http header.
    /// </summary>
    public class HttpHeader
    {
        /// <summary>
        /// Gets a Boolean indicating whether this http header is empty.
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(Name);
        /// <summary>
        /// Gets a Boolean indicating whether this http header has a value.
        /// </summary>
        public bool HasValue => Value != null;

        /// <summary>
        /// Gets or sets the name of this HttpHeader.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value of this HttpHeader.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new HttpHeader which is empty
        /// </summary>
        public HttpHeader()
        {
        }

        /// <summary>
        /// Initializes a new HttpHeader using the given header string.
        /// </summary>
        /// <param name="header">A string value in with the format of name: value;.</param>
        public HttpHeader(string header)
        {
            if (string.IsNullOrEmpty(header)) throw new ArgumentNullException(nameof(header));

            var pair = RegexHelper.ParseHttpHeader(header);

            if (pair == null) throw new FormatException("Wrong HttpHeader format");

            Name = pair.Item1;
            Value = pair.Item2;
        }

        /// <summary>
        /// Initializes a new HttpHeader using the given name and optiónal value.
        /// </summary>
        /// <param name="name">The string value determining the http header name.</param>
        /// <param name="value">The string value of the http header.</param>
        public HttpHeader(string name, string value = "")
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Converts this HttpHeader into a well formatted http header.
        /// </summary>
        /// <returns>The string this method creates.</returns>
        public override string ToString()
        {
            if (IsEmpty) throw new InvalidOperationException("HttpHeader is empty");

            if (HasValue)
            {
                return Name + ": " + Value;
            }
            else
            {
                return Name + ": ";
            }
        }
    }
}
