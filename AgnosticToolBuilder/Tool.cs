using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AnthropicApp
{


    public class Tool
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? description { get; set; }

        [JsonPropertyName("input_schema")]
        public InputSchema input_schema { get; set; }

        // This 'type' is for the tool definition, e.g., "custom" or "web_search_20250305"
        // It should always be serialized for Anthropic tools.
        [JsonPropertyName("type")]
        public string ToolDefinitionType { get; set; }
 

        [JsonPropertyName("cache_control")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CacheControl? cache_control { get; set; }
 


        // Constructor for standard/custom client-side tools
        public Tool(string name, string description, InputSchema inputSchema, CacheControl? cacheControl = null)
        {
            this.name = name;
            this.description = description;
            this.input_schema = inputSchema;
            this.cache_control = cacheControl;
            this.ToolDefinitionType = "custom"; // *** FIXED: Set to "custom" for client-side tools ***
        }

        // Constructor for tools with specific types (like web_search) and configurations
        // This constructor is used by LoadToolClass for the web_search tool.
        //public Tool(string name, string Description, InputSchema inputSchema, string toolDefinitionType,
        //            int? maxUses = null, List<string>? allowedDomains = null, List<string>? blockedDomains = null,
        //            UserLocation? userLocation = null, CacheControl? cacheControl = null)
        //{
        //    this.name = name;
        //    this.Description = Description;
        //    this.input_schema = inputSchema;
        //    this.ToolDefinitionType = toolDefinitionType; // This will be "web_search_20250305"
        //    this.max_uses = maxUses;
        //    this.AllowedDomains = allowedDomains;
        //    this.BlockedDomains = blockedDomains;
        //    this.UserLocation = userLocation;
        //    this.cache_control = cacheControl;
        //}
    }
}