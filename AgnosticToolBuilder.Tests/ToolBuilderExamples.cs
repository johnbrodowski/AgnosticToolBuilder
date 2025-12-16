using System.Text.Json;
using AnthropicApp;
using Xunit;

namespace AgnosticToolBuilder.Tests
{
    /// <summary>
    /// Examples demonstrating how to use the AgnosticToolBuilder library.
    /// These tests serve as both documentation and verification.
    /// </summary>
    public class ToolBuilderExamples
    {
        /// <summary>
        /// Example 1: Building a simple tool with Anthropic builder
        /// </summary>
        [Fact]
        public void Example1_BuildSimpleAnthropicTool()
        {
            // Create a simple tool for reading files
            var tool = new ToolTransformerBuilderAnthropic()
                .AddToolName("read_file")
                .AddDescription("Reads the contents of a file from the filesystem")
                .AddProperty(
                    fieldName: "file_path",
                    fieldType: "string",
                    fieldDescription: "The full path to the file to read",
                    isRequired: true
                )
                .Build();

            // Verify the tool was built correctly
            Assert.Equal("read_file", tool.name);
            Assert.Contains("Reads the contents", tool.description);
            Assert.NotNull(tool.input_schema);
            Assert.Single(tool.input_schema.required);
        }

        /// <summary>
        /// Example 2: Building a tool with nested objects
        /// </summary>
        [Fact]
        public void Example2_BuildToolWithNestedObjects()
        {
            var tool = new ToolTransformerBuilderAnthropic()
                .AddToolName("create_user")
                .AddDescription("Creates a new user account in the system")
                .AddNestedObject("user_data", "User information", isRequired: true)
                    .AddProperty("username", "string", "User's login name", isRequired: true)
                    .AddProperty("email", "string", "User's email address", isRequired: true)
                    .AddProperty("age", "integer", "User's age", isRequired: false)
                .EndNestedObject()
                .EndObject()
                .Build();

            Assert.Equal("create_user", tool.name);
            Assert.NotNull(tool.input_schema);

            // Verify the nested object exists
            Assert.True(tool.input_schema.properties.ContainsKey("user_data"));
        }

        /// <summary>
        /// Example 3: Building a tool with keywords, constraints, and instructions
        /// </summary>
        [Fact]
        public void Example3_BuildToolWithMetadata()
        {
            var tool = new ToolTransformerBuilderAnthropic()
                .AddToolName("execute_code")
                .AddDescription("Executes code in a sandboxed environment")
                .AddKeyWords("Code Execution", "Sandbox", "Security")
                .AddConstraint("Code must be syntactically valid")
                .AddConstraint("Execution time limited to 30 seconds")
                .AddInstructionHeader("Code Execution Guidelines")
                .AddInstructions("Always validate code before execution")
                .AddInstructions("Check for malicious patterns")
                .AddProperty("code", "string", "The code to execute", isRequired: true)
                .AddProperty("language", "string", "Programming language (python, javascript, etc.)", isRequired: true)
                .Build();

            Assert.Equal("execute_code", tool.name);
            Assert.Contains("Keywords:", tool.description);
            Assert.Contains("Constraints:", tool.description);
            Assert.Contains("Instructions:", tool.description);
        }

        /// <summary>
        /// Example 4: Using UniversalToolBuilder to create once, convert to any provider
        /// </summary>
        [Fact]
        public void Example4_UniversalToolBuilder()
        {
            // Define the tool ONCE using UniversalToolBuilder
            var universalTool = new UniversalToolBuilder()
                .AddToolName("send_email")
                .AddDescription("Sends an email to specified recipients")
                .AddKeyWords("Communication", "Email")
                .AddNestedObject("email_config", "Email configuration", isRequired: true)
                    .AddProperty("to", "string", "Recipient email address", isRequired: true)
                    .AddProperty("subject", "string", "Email subject line", isRequired: true)
                    .AddProperty("body", "string", "Email body content", isRequired: true)
                .EndNestedObject()
                .EndObject()
                .Build();

            // Convert to different provider formats
            var anthropicTool = universalTool.ToAnthropic();
            var openAiTool = universalTool.ToOpenAI();
            var geminiTool = universalTool.ToGemini();
            var groqTool = universalTool.ToGroq();
            var lmStudioTool = universalTool.ToLMStudio();

            // Verify all tools have the same name
            Assert.Equal("send_email", anthropicTool.name);
            Assert.Equal("send_email", openAiTool.name);
            Assert.Equal("send_email", geminiTool.name);
            Assert.Equal("send_email", groqTool.Function.Name);
            Assert.Equal("send_email", lmStudioTool.function.name);
        }

        /// <summary>
        /// Example 5: Building OpenAI tool with strict mode
        /// </summary>
        [Fact]
        public void Example5_BuildOpenAIToolWithStrictMode()
        {
            var tool = new ToolTransformerBuilderOpenAI()
                .AddToolName("get_weather")
                .AddDescription("Gets current weather for a location")
                .SetStrict(true)
                .SetAdditionalProperties(false)
                .AddProperty("location", "string", "City name", isRequired: true)
                .AddProperty("units", "string", "Temperature units (celsius/fahrenheit)", isRequired: false)
                .Build();

            Assert.Equal("get_weather", tool.name);
            Assert.True(tool.strict);
        }

        /// <summary>
        /// Example 6: Serializing a tool to JSON
        /// </summary>
        [Fact]
        public void Example6_SerializeToolToJson()
        {
            var tool = new ToolTransformerBuilderAnthropic()
                .AddToolName("calculate")
                .AddDescription("Performs mathematical calculations")
                .AddProperty("expression", "string", "Mathematical expression to evaluate", isRequired: true)
                .Build();

            // Serialize to JSON
            var json = ToolStringOutput.GenerateToolJson(tool);

            // Verify JSON contains expected data
            Assert.Contains("\"name\": \"calculate\"", json);
            Assert.Contains("\"expression\"", json);

            // Verify it's valid JSON
            var parsed = JsonSerializer.Deserialize<JsonDocument>(json);
            Assert.NotNull(parsed);
        }

        /// <summary>
        /// Example 7: Building a tool with array properties
        /// </summary>
        [Fact]
        public void Example7_BuildToolWithArrays()
        {
            var tool = new ToolTransformerBuilderAnthropic()
                .AddToolName("batch_process")
                .AddDescription("Processes multiple files in batch")
                .AddProperty(
                    fieldName: "file_paths",
                    fieldType: "array",
                    fieldDescription: "List of file paths to process",
                    isRequired: true,
                    items: new Dictionary<string, string> { { "type", "string" } }
                )
                .Build();

            Assert.Equal("batch_process", tool.name);
            Assert.NotNull(tool.input_schema.properties["file_paths"]);
        }

        /// <summary>
        /// Example 8: Building a complex tool with multiple nested objects
        /// </summary>
        [Fact]
        public void Example8_ComplexNestedStructure()
        {
            var tool = new ToolTransformerBuilderAnthropic()
                .AddToolName("create_project")
                .AddDescription("Creates a new project with configuration")
                .AddNestedObject("project_config", "Project configuration", isRequired: true)
                    .AddProperty("name", "string", "Project name", isRequired: true)
                    .AddProperty("description", "string", "Project description", isRequired: false)
                    .AddNestedObject("database", "Database settings", isRequired: true)
                        .AddProperty("host", "string", "Database host", isRequired: true)
                        .AddProperty("port", "integer", "Database port", isRequired: true)
                        .AddProperty("name", "string", "Database name", isRequired: true)
                    .EndNestedObject()
                    .AddNestedObject("api", "API settings", isRequired: false)
                        .AddProperty("base_url", "string", "API base URL", isRequired: true)
                        .AddProperty("timeout", "integer", "Request timeout in seconds", isRequired: false)
                    .EndNestedObject()
                .EndNestedObject()
                .EndObject()
                .Build();

            Assert.Equal("create_project", tool.name);
            Assert.NotNull(tool.input_schema);
        }

        /// <summary>
        /// Example 9: Demonstrates all 5 provider-specific builders
        /// </summary>
        [Fact]
        public void Example9_AllProviderBuilders()
        {
            // Anthropic
            var anthropicTool = new ToolTransformerBuilderAnthropic()
                .AddToolName("example_tool")
                .AddDescription("Example tool")
                .Build();

            // OpenAI
            var openAiTool = new ToolTransformerBuilderOpenAI()
                .AddToolName("example_tool")
                .AddDescription("Example tool")
                .Build();

            // Gemini
            var geminiTool = new ToolTransformerBuilderGemini()
                .AddToolName("example_tool")
                .AddDescription("Example tool")
                .Build();

            // Groq
            var groqTool = new ToolTransformerBuilderGroq()
                .AddToolName("example_tool")
                .AddDescription("Example tool")
                .Build();

            // LMStudio
            var lmStudioTool = new ToolTransformerBuilderLMStudio()
                .AddToolName("example_tool")
                .AddDescription("Example tool")
                .Build();

            // All builders work with the same API
            Assert.Equal("example_tool", anthropicTool.name);
            Assert.Equal("example_tool", openAiTool.name);
            Assert.Equal("example_tool", geminiTool.name);
            Assert.Equal("example_tool", groqTool.Function.Name);
            Assert.Equal("example_tool", lmStudioTool.function.name);
        }
    }
}
