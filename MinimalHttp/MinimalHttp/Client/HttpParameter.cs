using System;
using System.Text;

namespace MinimalHttp.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpParameter
    {
        /// <summary>
        /// The delimeters
        /// </summary>
        private static string[] delimeters = new string[] { "=", ":", "|", ",", " "};
        /// <summary>
        /// The default delimeter
        /// </summary>
        public static string DefaultDelimeter = "=";

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(Key)) return true;
                if (Value == null) return true;

                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpParameter"/> class.
        /// </summary>
        public HttpParameter()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpParameter"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public HttpParameter(string data)
        {
            ParseData(data, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpParameter"/> class.
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
        /// Initializes a new instance of the <see cref="HttpParameter"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="ArgumentNullException">key</exception>
        public HttpParameter(string key, params string[] values)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            Key = key;

            if (!Key.Contains("[]")) Key += "[]";

            if(values == null || values.Length == 0)
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
        /// Parses the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="override_delimter">The override delimter.</param>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="FormatException">
        /// The given string \"" + nameof(data) + "\" does not contain any supported delimeter!
        /// or
        /// The string \"" + nameof(data) + "\" could not be splitted by \"" + used_delimeter + "\"!
        /// or
        /// The parsed data doesn't contain any values!
        /// </exception>
        private void ParseData(string data, string override_delimter)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data));

            string used_delimeter = null;

            if (override_delimter == null)
            {
                foreach (var delimeter in delimeters)
                {
                    if (!data.Contains(delimeter)) continue;

                    used_delimeter = delimeter;

                    break;
                }

                if (string.IsNullOrEmpty(used_delimeter))
                {
                    used_delimeter = DefaultDelimeter;
                }
            }
            else
            {
                used_delimeter = override_delimter;
            }

            if (!data.Contains(used_delimeter)) throw new FormatException("The given string \"" + nameof(data) + "\" does not contain any supported delimeter!");

            string[] pair = data.Split(used_delimeter.ToCharArray());

            if(pair == null)
            {
                throw new FormatException("The string \"" + nameof(data) + "\" could not be splitted by \"" + used_delimeter + "\"!");
            }
            else if(pair.Length == 0)
            {
                throw new FormatException("The parsed data doesn't contain any values!");
            }
            else if(pair.Length == 1)
            {
                Key = pair[0];
                Value = "";
            }
            else if(pair.Length == 2)
            {
                Key = pair[0];
                Value = pair[1];
            }
            else
            {
                Key = pair[0] + "[]";

                for (int i = 1; i < pair.Length - 1; i++)
                    Value += pair[i] + ",";

                Value += pair[pair.Length - 1];
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if(obj is HttpParameter)
            {
                var param = (HttpParameter)obj;
                
                return this.Key == param.Key && this.Value == param.Value;
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if(this.IsEmpty && this.Value != null)
            {
                return this.Value.GetHashCode();
            }
            else if(!this.IsEmpty)
            {
                return this.Key.GetHashCode() ^ this.Value.GetHashCode();
            }

            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.IsEmpty) return string.Empty;

            if(this.Value == null) return string.Concat(this.Key, "=", string.Empty);

            if(this.Key.Contains("[]"))
            {
                StringBuilder builder = new StringBuilder(this.Key.Length + this.Value.Length);

                foreach (var value in this.Value.Split(','))
                {
                    builder.Append(this.Key);
                    builder.Append("=");
                    builder.Append(value);
                }

                return builder.ToString();
            }
            else
            {
                return string.Concat(this.Key, "=", this.Value);
            }
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.String"/> to <see cref="HttpParameter"/>.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator HttpParameter(string data)
        {
            return new HttpParameter(data);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="HttpParameter"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator string(HttpParameter parameter)
        {
            return parameter.ToString();
        }
    }
}
