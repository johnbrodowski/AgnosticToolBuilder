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
 
    }
}