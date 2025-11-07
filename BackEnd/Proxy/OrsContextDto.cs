using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Proxy
{
    [DataContract]
    public class PositionDto
    {
        [JsonPropertyName("x")]
        [DataMember] public double Longitude { get; set; }

        [JsonPropertyName("y")]
        [DataMember] public double Latitude { get; set; }

    }

    public class OrsContextDto
    {
        [JsonPropertyName("startPosition")]
        public PositionDto StartPosition { get; set; }

        [JsonPropertyName("endPosition")]
        public PositionDto EndPosition { get; set; }

        [JsonPropertyName("profile")]
        public string Profile { get; set; }

    }
}
