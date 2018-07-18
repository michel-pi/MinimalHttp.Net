using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalHttp.Client
{
    /// <summary>
    /// 
    /// </summary>
    public enum HttpRequestMethod
    {
        /// <summary>
        /// The unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// The get
        /// </summary>
        Get,
        /// <summary>
        /// The post
        /// </summary>
        Post,
        /// <summary>
        /// The head
        /// </summary>
        Head,
        /// <summary>
        /// The put
        /// </summary>
        Put,
        /// <summary>
        /// The delete
        /// </summary>
        Delete
    }
}
