﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Position
    {
        public Position()
        {

        }

        public Position(long line, long character)
        {
            Line = line;
            Character = character;
        }
        /// <summary>
        /// Line position in a document (zero-based).
        /// </summary>
        public long Line { get; set; }

        /// <summary>
        /// Character offset on a line in a document (zero-based).
        /// </summary>
        public long Character { get; set; }
    }
}