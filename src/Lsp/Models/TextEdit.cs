using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextEdit
    {
        /// <summary>
        /// The range of the text document to be manipulated. To insert
        /// text into a document create a range where start === end.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The string to be inserted. For delete operations use an
        /// empty string.
        /// </summary>
        public string NewText { get; set; }
    }
}