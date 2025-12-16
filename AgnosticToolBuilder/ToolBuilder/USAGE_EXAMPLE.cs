using System;
using System.Collections.Generic;

namespace AnthropicApp
{
    /// <summary>
    /// Example demonstrating how to use UniversalToolBuilder to define tools once
    /// and convert them to any provider format.
    /// All 5 providers (Anthropic, OpenAI, Gemini, Groq, LMStudio) have identical interfaces.
    /// </summary>
    public class UniversalToolBuilderExample
    {
        public static void Example()
        {
            // ========================================================================
            // BEFORE: You had to define the same tool for each provider separately
            // ========================================================================

            // For Anthropic
            var anthropicTool = new ToolTransformerBuilderAnthropic()
                .AddToolName("get_open_windows")
                .AddDescription("Retrieves a list of open windows.")
                .AddNestedObject("get_open_windows_params", "Request for a list of open windows.", isRequired: true)
                    .AddProperty("name", "string", "Specifies the window name", isRequired: false)
                .EndNestedObject()
                .EndObject()
                .Build();

            // For OpenAI
            var openAiTool = new ToolTransformerBuilderOpenAI()
                .AddToolName("get_open_windows")
                .AddDescription("Retrieves a list of open windows.")
                .AddNestedObject("get_open_windows_params", "Request for a list of open windows.", isRequired: true)
                    .AddProperty("name", "string", "Specifies the window name", isRequired: false)
                .EndNestedObject()
                .EndObject()
                .Build();

            // And so on for Gemini, Groq, LMStudio... very repetitive!


            // ========================================================================
            // AFTER: Define once, convert to any provider
            // ========================================================================

            // 1. Define the tool ONCE using UniversalToolBuilder
            var universalTool = new UniversalToolBuilder()
                .AddToolName("get_open_windows")
                .AddDescription("Retrieves a list of open windows.")
                .AddNestedObject("get_open_windows_params", "Request for a list of open windows.", isRequired: true)
                    .AddProperty("name", "string", "Specifies the window name", isRequired: false)
                .EndNestedObject()
                .EndObject()
                .Build();

            // 2. Convert to ANY provider format with a single method call
            var anthropicToolConverted = universalTool.ToAnthropic();
            var openAiToolConverted = universalTool.ToOpenAI();
            var geminiTool = universalTool.ToGemini();
            var groqTool = universalTool.ToGroq();
            var lmStudioTool = universalTool.ToLMStudio();


            // ========================================================================
            // ADVANCED EXAMPLE: Tool with all features
            // ========================================================================

            var advancedTool = new UniversalToolBuilder()
                .AddToolName("file_operations")
                .AddDescription("Performs various file system operations.")
                .AddKeyWords("File System", "Storage", "I/O")
                .AddConstraint("User must have write permissions")
                .AddConstraint("Path must be within allowed directories")
                .AddInstructionHeader("File Operation Guidelines")
                .AddInstructions("Always validate paths before operations")
                .AddInstructions("Check file existence before reading")
                .SetStrict(true)
                .SetAdditionalProperties(false)
                .AddNestedObject("operation_config", "Configuration for the file operation", isRequired: true)
                    .AddProperty("operation_type", "string", "Type of operation: read, write, delete", isRequired: true)
                    .AddProperty("file_path", "string", "Full path to the target file", isRequired: true)
                    .AddProperty("content", "string", "Content to write (for write operations)", isRequired: false)
                    .AddNestedObject("options", "Additional operation options", isRequired: false)
                        .AddProperty("create_if_missing", "boolean", "Create file if it doesn't exist", isRequired: false)
                        .AddProperty("overwrite", "boolean", "Overwrite existing file", isRequired: false)
                    .EndNestedObject()
                .EndNestedObject()
                .EndObject()
                .Build();

            // Convert to all providers
            var anthropicAdvanced = advancedTool.ToAnthropic();
            var openAiAdvanced = advancedTool.ToOpenAI();
            var geminiAdvanced = advancedTool.ToGemini();
            var groqAdvanced = advancedTool.ToGroq();
            var lmStudioAdvanced = advancedTool.ToLMStudio();


            // ========================================================================
            // MIGRATION STRATEGY: You can migrate gradually
            // ========================================================================

            // Old code still works - no breaking changes!
            var oldStyleTool = new ToolTransformerBuilderAnthropic()
                .AddToolName("legacy_tool")
                .AddDescription("This still works!")
                .Build();

            // New code can use universal builder
            var newStyleTool = new UniversalToolBuilder()
                .AddToolName("modern_tool")
                .AddDescription("This is the new way!")
                .Build()
                .ToAnthropic();

            // Both approaches work side by side - migrate at your own pace!
        }


        // ========================================================================
        // PRACTICAL EXAMPLE: Reusable tool definitions
        // ========================================================================

        /// <summary>
        /// Create a reusable tool definition that can be used across all providers
        /// </summary>
        public static UniversalToolDefinition CreateFileReadTool()
        {
            return new UniversalToolBuilder()
                .AddToolName("read_file")
                .AddDescription("Retrieves and displays file content from the file system.")
                .AddKeyWords("Content Retrieval", "File Inspection", "Data Access")
                .AddConstraint("Target file must exist in the specified location")
                .AddInstructionHeader("File Reading Guidelines")
                .AddInstructions("Verify file existence before attempting to read")
                .AddInstructions("Use for quick content inspection without editing needs")
                .AddNestedObject("file_class", "File access parameters", isRequired: true)
                    .AddProperty("file_path", "string", "Full path to the target file", isRequired: true)
                .EndNestedObject()
                .EndObject()
                .Build();
        }

        /// <summary>
        /// Use the reusable tool definition for any provider
        /// </summary>
        public static void UseReusableTool()
        {
            var fileReadTool = CreateFileReadTool();

            // Use with any provider
            var forAnthropic = fileReadTool.ToAnthropic();
            var forOpenAI = fileReadTool.ToOpenAI();
            var forGemini = fileReadTool.ToGemini();
            var forGroq = fileReadTool.ToGroq();
            var forLMStudio = fileReadTool.ToLMStudio();
        }
    }
}
