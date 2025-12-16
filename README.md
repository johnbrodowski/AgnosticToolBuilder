# AgnosticToolBuilder

A provider-agnostic tool builder for AI/LLM function calling that lets you define tools once and use them across multiple AI providers (Anthropic, OpenAI, Gemini, Groq, LMStudio).

## Features

- 🎯 **Define once, use everywhere** - Write tool definitions once and convert to any provider format
- 🔧 **Fluent API** - Clean, readable builder pattern with method chaining
- 🏗️ **Nested object support** - Build complex tool schemas with nested structures
- 📝 **Rich metadata** - Add keywords, constraints, and instructions to guide AI behavior
- ✅ **Type-safe** - Full C# type safety with compile-time checking
- 🚀 **Five providers supported** - Anthropic, OpenAI, Gemini, Groq, and LMStudio

## Installation

Add the project reference to your .csproj:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/AgnosticToolBuilder/AgnosticToolBuilder.csproj" />
</ItemGroup>
```

## Quick Start

### Option 1: Provider-Specific Builder (Direct)

```csharp
using AnthropicApp;

// Build a tool specifically for Anthropic
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

// Use with Anthropic API
// var response = await anthropicClient.Messages.CreateAsync(new MessageRequest
// {
//     Tools = new[] { tool },
//     ...
// });
```

### Option 2: Universal Builder (Convert to Any Provider)

```csharp
using AnthropicApp;

// Define ONCE using UniversalToolBuilder
var universalTool = new UniversalToolBuilder()
    .AddToolName("send_email")
    .AddDescription("Sends an email to specified recipients")
    .AddProperty("to", "string", "Recipient email", isRequired: true)
    .AddProperty("subject", "string", "Email subject", isRequired: true)
    .AddProperty("body", "string", "Email body", isRequired: true)
    .Build();

// Convert to ANY provider format
var anthropicTool = universalTool.ToAnthropic();
var openAiTool = universalTool.ToOpenAI();
var geminiTool = universalTool.ToGemini();
var groqTool = universalTool.ToGroq();
var lmStudioTool = universalTool.ToLMStudio();
```

## Examples

### 1. Simple Tool with Basic Properties

```csharp
var tool = new ToolTransformerBuilderAnthropic()
    .AddToolName("get_weather")
    .AddDescription("Gets current weather for a location")
    .AddProperty("location", "string", "City name", isRequired: true)
    .AddProperty("units", "string", "Temperature units", isRequired: false)
    .Build();
```

### 2. Tool with Nested Objects

```csharp
var tool = new ToolTransformerBuilderAnthropic()
    .AddToolName("create_user")
    .AddDescription("Creates a new user account")
    .AddNestedObject("user_data", "User information", isRequired: true)
        .AddProperty("username", "string", "User's login name", isRequired: true)
        .AddProperty("email", "string", "User's email", isRequired: true)
        .AddProperty("age", "integer", "User's age", isRequired: false)
    .EndNestedObject()
    .EndObject()
    .Build();
```

### 3. Tool with Keywords, Constraints, and Instructions

```csharp
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
    .AddProperty("language", "string", "Programming language", isRequired: true)
    .Build();
```

### 4. Tool with Arrays

```csharp
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
```

### 5. Complex Multi-Level Nested Objects

```csharp
var tool = new ToolTransformerBuilderAnthropic()
    .AddToolName("create_project")
    .AddDescription("Creates a new project with configuration")
    .AddNestedObject("project_config", "Project configuration", isRequired: true)
        .AddProperty("name", "string", "Project name", isRequired: true)
        .AddNestedObject("database", "Database settings", isRequired: true)
            .AddProperty("host", "string", "Database host", isRequired: true)
            .AddProperty("port", "integer", "Database port", isRequired: true)
        .EndNestedObject()
        .AddNestedObject("api", "API settings", isRequired: false)
            .AddProperty("base_url", "string", "API base URL", isRequired: true)
        .EndNestedObject()
    .EndNestedObject()
    .EndObject()
    .Build();
```

### 6. OpenAI with Strict Mode

```csharp
var tool = new ToolTransformerBuilderOpenAI()
    .AddToolName("get_weather")
    .AddDescription("Gets current weather")
    .SetStrict(true)  // OpenAI strict mode
    .SetAdditionalProperties(false)
    .AddProperty("location", "string", "City name", isRequired: true)
    .Build();
```

## Running the Tests

The test project contains working examples you can run:

```bash
# Run all tests
dotnet test

# Run a specific test
dotnet test --filter "Example1_BuildSimpleAnthropicTool"
```

Each test demonstrates a different feature and serves as both documentation and verification.

## API Reference

### Builder Methods

All builders (Anthropic, OpenAI, Gemini, Groq, LMStudio, Universal) support:

| Method | Description |
|--------|-------------|
| `AddToolName(string)` | Set the tool name (required) |
| `AddDescription(string)` | Set the tool description |
| `AddProperty(...)` | Add a simple property to the tool |
| `AddNestedObject(...)` | Start defining a nested object |
| `EndNestedObject()` | Return to parent nested object |
| `EndObject()` | Return to main tool builder |
| `AddKeyWords(params string[])` | Add categorization keywords |
| `AddConstraint(params string[])` | Add usage constraints |
| `AddInstructionHeader(string)` | Set header for instructions section |
| `AddInstructions(string)` | Add a usage instruction |
| `Build()` | Build the final tool object |

### Provider-Specific Methods

**OpenAI, Groq:**
- `SetStrict(bool)` - Enable strict mode
- `SetAdditionalProperties(bool)` - Allow additional properties

**Gemini:**
- `SetStrict(bool)` - Enable strict mode

**LMStudio:**
- `SetAdditionalProperties(bool)` - Allow additional properties

### Universal Builder Conversion

```csharp
var universal = new UniversalToolBuilder()...Build();

// Convert to provider-specific formats
var anthropic = universal.ToAnthropic();
var openai = universal.ToOpenAI();
var gemini = universal.ToGemini();
var groq = universal.ToGroq();
var lmstudio = universal.ToLMStudio();
```

## Serialization to JSON

```csharp
var tool = new ToolTransformerBuilderAnthropic()
    .AddToolName("example")
    .AddDescription("Example tool")
    .Build();

// Serialize to JSON string
var json = ToolStringOutput.GenerateToolJson(tool);
Console.WriteLine(json);
```

Output:
```json
{
  "name": "example",
  "description": "Example tool",
  "input_schema": {
    "type": "object",
    "properties": {},
    "required": null
  },
  "type": "custom"
}
```

## Property Types

Supported property types:
- `"string"` - Text values
- `"integer"` - Whole numbers
- `"number"` - Decimal numbers
- `"boolean"` - True/false values
- `"array"` - Lists (requires `items` parameter)
- `"object"` - Use `AddNestedObject()` instead

## Migration from Existing Code

If you have existing provider-specific tool definitions:

**Before (repetitive):**
```csharp
var anthropicTool = new ToolTransformerBuilderAnthropic()...Build();
var openAiTool = new ToolTransformerBuilderOpenAI()...Build();
var geminiTool = new ToolTransformerBuilderGemini()...Build();
// Same definition repeated 5 times!
```

**After (define once):**
```csharp
var universal = new UniversalToolBuilder()...Build();
var anthropicTool = universal.ToAnthropic();
var openAiTool = universal.ToOpenAI();
var geminiTool = universal.ToGemini();
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for your changes
4. Ensure all tests pass
5. Submit a pull request

## License

See [LICENSE.txt](LICENSE.txt) for details.

## Support

- Report issues: [GitHub Issues](https://github.com/johnbrodowski/AgnosticToolBuilder/issues)
- View examples: See `AgnosticToolBuilder.Tests/ToolBuilderExamples.cs`
