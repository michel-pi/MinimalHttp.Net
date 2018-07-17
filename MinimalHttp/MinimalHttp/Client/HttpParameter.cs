using System;
using System.Text;

namespace MinimalHttp.Client
{
    public class HttpParameter
    {
        private static string[] delimeters = new string[] { "=", ":", "|", ",", " "};
        public static string DefaultDelimeter = "=";

        public string Key { get; set; }
        public string Value { get; set; }

        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(Key)) return true;
                if (Value == null) return true;

                return false;
            }
        }

        public HttpParameter()
        {

        }

        public HttpParameter(string data)
        {
            ParseData(data, null);
        }

        public HttpParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            Key = key;
            Value = value ?? "";
        }

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

        public override bool Equals(object obj)
        {
            if(obj is HttpParameter)
            {
                var param = (HttpParameter)obj;
                
                return this.Key == param.Key && this.Value == param.Value;
            }

            return base.Equals(obj);
        }

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

        public static explicit operator HttpParameter(string data)
        {
            return new HttpParameter(data);
        }

        public static explicit operator string(HttpParameter parameter)
        {
            return parameter.ToString();
        }
    }
}
