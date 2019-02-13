using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MinimalHttp
{
    /// <summary>
    /// Represents a single http parameter.
    /// </summary>
    public class HttpParameter
    {
        private const string HttpDelimeter = "=";
        private const string HttpArray = "[]";

        private static readonly string[] SupportedDelimeters = { "=", ":", "|", ",", " " };

        /// <summary>
        /// A Boolean indicating whether this parameter has a value.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(Value);
        /// <summary>
        /// A Boolean indicating whether this parameter is an array.
        /// </summary>
        public bool IsArray => Name.EndsWith(HttpArray);

        /// <summary>
        /// Gets or sets the name of this HttpParameter.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value of this HttpParameter.
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// Initializes a new HttpParameter which is empty.
        /// </summary>
        public HttpParameter()
        {
        }

        /// <summary>
        /// Initializes a new HttpParameter and parses the given data.
        /// </summary>
        /// <param name="data">A data string containg key value pairs seperated by =,:,|,  or ,.</param>
        public HttpParameter(string data)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data));

            var pair = ParseDataString(data);

            Name = pair.Item1;
            Value = pair.Item2;
        }

        /// <summary>
        /// Initializes a new HttpParameter using the given name and value.
        /// </summary>
        /// <param name="name">A string representing the name of this HttpParameter.</param>
        /// <param name="value">A string representing the value of this HttpParameter.</param>
        public HttpParameter(string name, string value = "")
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            
            Name = name;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Initializes a new HttpParameter with a value array by using the given name and value array.
        /// </summary>
        /// <param name="name">A string representing the name of this HttpParameter.</param>
        /// <param name="values">An array of string values.</param>
        public HttpParameter(string name, params string[] values)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) throw new ArgumentOutOfRangeException(nameof(values));

            if (name.EndsWith(HttpArray))
            {
                Name = name;
            }
            else
            {
                Name = name + HttpArray;
            }
            
            Value = CreateHttpValueArray(values);
        }

        /// <summary>
        /// Converts this HttpParameter to a well formatted http parameter string.
        /// </summary>
        /// <returns>The string this method creates.</returns>
        public override string ToString()
        {
            return Name + HttpDelimeter + Value;
        }

        private static Tuple<string, string> ParseDataString(string data)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data));

            string usedDelimeter = string.Empty;

            foreach (var delimeter in SupportedDelimeters)
            {
                if (data.Contains(delimeter))
                {
                    usedDelimeter = delimeter;
                    break;
                }
            }

            if (string.IsNullOrEmpty(usedDelimeter))
            {
                throw new FormatException(nameof(data));
            }
            else
            {
                var split = data.Split(usedDelimeter[0]);

                if (split.Length == 0)
                {
                    throw new FormatException(nameof(data));
                }
                else if (split.Length == 1)
                {
                    return new Tuple<string, string>(split[0], string.Empty);
                }
                else if (split.Length == 2)
                {
                    return new Tuple<string, string>(split[0], split[1]);
                }
                else
                {
                    return new Tuple<string, string>(split[0] + HttpArray, CreateHttpValueArray(split.Skip(1)));
                }
            }
        }

        private static string CreateHttpValueArray(IEnumerable<string> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var sb = new StringBuilder();

            foreach (var value in values)
                sb.Append(value + ",");

            sb.Length--; // removes the last ","

            return sb.ToString();
        }
    }
}
