using System.Text.Json.Serialization;

namespace AnthropicApp
{
    public class ParametersGemini
    {
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? type { get; set; }

        [JsonPropertyName("properties")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? properties { get; set; }

        //[JsonPropertyName("additionalProperties")]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
        //public bool? additionalProperties { get; set; } = null;

        [JsonPropertyName("required")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? required { get; set; }

        public ParametersGemini(string type, Dictionary<string, object> properties, List<string> required = null/*, bool? additionalProperties = null*/)
        {
            this.type = type; this.properties = properties; /*this.additionalProperties = additionalProperties;*/ this.required = required;
        }
    }

}
 