using System;
using System.Text;
using System.Collections.Generic;

namespace MinimalHttp.Utilities
{
    /// <summary>
    /// Provides a container for a http resource and its url query parameters.
    /// </summary>
    public class UrlQueryBuilder
    {
        /// <summary>
        /// Gets or sets the URL of the used resource.
        /// </summary>
        public string Resource { get; private set; }
        /// <summary>
        /// Gets the list of HttpParameter used by this instance.
        /// </summary>
        public List<HttpParameter> Parameters { get; private set; }

        /// <summary>
        /// Initializes a new empty UrlQueryBuilder.
        /// </summary>
        public UrlQueryBuilder()
        {
            Resource = string.Empty;
            Parameters = new List<HttpParameter>();
        }

        /// <summary>
        /// Initializes a new UrlQueryBuilder using the given resource url.
        /// </summary>
        /// <param name="resource">A valid and absolut url string.</param>
        public UrlQueryBuilder(string resource)
        {
            if (string.IsNullOrEmpty(resource)) throw new ArgumentNullException(nameof(resource));

            Resource = resource;
            Parameters = new List<HttpParameter>();
        }

        /// <summary>
        /// Adds a HttpParameter to the Parameters list
        /// </summary>
        /// <param name="parameter">The HttpParameter to add.</param>
        public void Add(HttpParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            Parameters.Add(parameter);
        }

        /// <summary>
        /// Converts this UrlQueryBuilder instance to a complete URL with it's query string attached.
        /// </summary>
        /// <returns>The URL string this method creates.</returns>
        public override string ToString()
        {
            if (Parameters == null) throw new ArgumentNullException(nameof(Parameters));
            if (Parameters.Count == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var parameter in Parameters)
            {
                if (parameter == null) continue;

                sb.Append(parameter.ToString() + "&");
            }

            sb.Length--;

            return sb.ToString();
        }
    }
}
