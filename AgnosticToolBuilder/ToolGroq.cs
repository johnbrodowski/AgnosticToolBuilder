using System.Text.Json.Serialization;

namespace AnthropicApp
{
    public partial class GroqClient
    {
        public class ToolGroq
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = "function";

            [JsonPropertyName("function")]
            public FunctionDefinitionGroq Function { get; set; } = new FunctionDefinitionGroq();
        }

        public class FunctionDefinitionGroq
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("description")]
            public string Description { get; set; } = string.Empty;

            [JsonPropertyName("parameters")]
            public ParametersGroq Parameters { get; set; } = new ParametersGroq();
        }
    }
}