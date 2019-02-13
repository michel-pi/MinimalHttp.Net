using System;
using System.Text;

namespace MinimalHttp.Client
{
    /// <summary>
    /// </summary>
    public class HttpParameter
    {
        /// <summary>
        ///     The delimeters
        /// </summary>
        private static readonly string[] Delimeters = {"=", ":", "|", ",", " "};

        /// <summary>
        ///     The default delimeter
        /// </summary>
        public static string DefaultDelimeter = "=";

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpParameter" /> class.
        /// </summary>
        public HttpParameter()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpParameter" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public HttpParameter(string data)
        {
            ParseData(data, null);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpParameter" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">key</exception>
        public HttpParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            Key = key;
            Value = value ?? "";
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpParameter" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="ArgumentNullException">key</exception>
        public HttpParameter(string key, params string[] values)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            Key = key;

            if (!Key.Contains("[]")) Key += "[]";

            if (values == null || values.Length == 0)
            {
                Value = "";
            }
            else
            {
                for (int i = 0; i < values.Length - 1; i++)
                    Value += values[i] + ",";

                Value += values[values.Length + 1];
            }
        }

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(Key)) return true;

                return Value == null;
            }
        }

        /// <summary>
        ///     Parses the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="overrideDelimter">The override delimter.</param>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="FormatException">
        ///     The given string \"" + nameof(data) + "\" does not contain any supported delimeter!
        ///     or
        ///     The string \"" + nameof(data) + "\" could not be splitted by \"" + used_delimeter + "\"!
        ///     or
        ///     The parsed data doesn't contain any values!
        /// </exception>
        private void ParseData(string data, string overrideDelimter)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data));

            string usedDelimeter = null;

            if (overrideDelimter == null)
            {
                foreach (string delimeter in Delimeters)
                {
                    if (!data.Contains(delimeter)) continue;

                    usedDelimeter = delimeter;

                    break;
                }

                if (string.IsNullOrEmpty(usedDelimeter))
                    usedDelimeter = DefaultDelimeter;
            }
            else
            {
                usedDelimeter = overrideDelimter;
            }

            if (!data.Contains(usedDelimeter))
                throw new FormatException("The given string \"" + nameof(data) +
                                          "\" does not contain any supported delimeter!");

            string[] pair = data.Split(usedDelimeter.ToCharArray());

            if (pair == null)
                throw new FormatException("The string \"" + nameof(data) + "\" could not be splitted by \"" +
                                          usedDelimeter + "\"!");
            switch (pair.Length)
            {
                case 0:
                    throw new FormatException("The parsed data doesn't contain any values!");
                case 1:
                    Key = pair[0];
                    Value = "";
                    break;
                case 2:
                    Key = pair[0];
                    Value = pair[1];
                    break;
                default:
                    Key = pair[0] + "[]";

                    for (int i = 1; i < pair.Length - 1; i++)
                        Value += pair[i] + ",";

                    Value += pair[pair.Length - 1];
                    break;
            }
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HttpParameter))
            {
                return base.Equals(obj);
            }

            var param = (HttpParameter) obj;

            return Key == param.Key && Value == param.Value;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            if (IsEmpty && Value != null)
                return Value.GetHashCode();
            if (!IsEmpty)
                return Key.GetHashCode() ^ Value.GetHashCode();

            return base.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (IsEmpty) return string.Empty;

            if (Value == null) return string.Concat(Key, "=", string.Empty);

            if (!Key.Contains("[]")) return string.Concat(Key, "=", Value);

            var builder = new StringBuilder(Key.Length + Value.Length);

            foreach (string value in Value.Split(','))
            {
                builder.Append(Key);
                builder.Append("=");
                builder.Append(value);
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="System.String" /> to <see cref="HttpParameter" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator HttpParameter(string data)
        {
            return new HttpParameter(data);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="HttpParameter" /> to <see cref="System.String" />.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator string(HttpParameter parameter)
        {
            return parameter.ToString();
        }
    }
}