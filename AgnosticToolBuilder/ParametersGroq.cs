using System.Text.Json.Serialization;

namespace AnthropicApp
{
    public partial class GroqClient
    {
        public class ParametersGroq
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = "object";

            [JsonPropertyName("properties")]
            public object Properties { get; set; } = new object();

            [JsonPropertyName("required")]
            public string[] Required { get; set; } = System.Array.Empty<string>();
        }
    }
}