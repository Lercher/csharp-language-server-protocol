using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    /// <summary>
    /// A document link is a range in a text document that links to an internal or external resource, like another
    /// text document or a web site.
    /// </summary>

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DocumentLink
    {
        /// <summary>
        /// The range this link applies to.
        /// </summary>
        public Range Range { get; set; }
        /// <summary>
        /// The uri this link points to. If missing a resolve request is sent later.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Uri Target { get; set; }
    }
}