﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonRpc.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Request
    {
        public object Id { get; set; }

        public string ProtocolVersion { get; } = "2.0";

        public string Method { get; set; }

        public object Params { get; set; }
    }
}