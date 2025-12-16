using System;
using System.Collections.Generic;
using System.Linq;

namespace AnthropicApp
{
    /// <summary>
    /// Converts UniversalToolDefinition to provider-specific tool formats.
    /// Reuses existing provider-specific builders internally.
    /// </summary>
    public static class ToolConverter
    {
        #region Anthropic

        public static Tool ToAnthropic(this UniversalToolDefinition definition)
        {
            var builder = new ToolTransformerBuilderAnthropic()
                .AddToolName(definition.Name)
                .AddDescription(definition.Description);

            ApplyMetadata(builder, definition);
            ApplyProperties(builder, definition);

            return builder.Build();
        }

        private static void ApplyMetadata(ToolTransformerBuilderAnthropic builder, UniversalToolDefinition definition)
        {
            if (definition.Keywords?.Count > 0)
                builder.AddKeyWords(definition.Keywords.ToArray());

            if (definition.Constraints?.Count > 0)
                builder.AddConstraint(definition.Constraints.ToArray());

            if (!string.IsNullOrEmpty(definition.InstructionHeader))
                builder.AddInstructionHeader(definition.InstructionHeader);

            if (definition.Instructions?.Count > 0)
            {
                foreach (var instruction in definition.Instructions)
                    builder.AddInstructions(instruction);
            }
        }

        private static void ApplyProperties(ToolTransformerBuilderAnthropic builder, UniversalToolDefinition definition)
        {
            foreach (var prop in definition.Properties)
            {
                AddPropertyToAnthropic(builder, prop.Key, prop.Value, definition.RequiredFields.Contains(prop.Key));
            }
        }

        private static void AddPropertyToAnthropic(ToolTransformerBuilderAnthropic builder, string name, UniversalProperty prop, bool isRequired)
        {
            if (prop.NestedProperties != null)
            {
                var nestedBuilder = builder.AddNestedObject(name, prop.Description, isRequired, prop.IsArray);
                AddNestedPropertiesToAnthropic(nestedBuilder, prop);
                nestedBuilder.EndObject();
            }
            else
            {
                builder.AddProperty(name, prop.Type, prop.Description, isRequired, prop.Items);
            }
        }

        private static void AddNestedPropertiesToAnthropic(NestedObjectBuilder builder, UniversalProperty prop)
        {
            foreach (var nested in prop.NestedProperties)
            {
                var isRequired = prop.RequiredFields?.Contains(nested.Key) ?? false;

                if (nested.Value.NestedProperties != null)
                {
                    var nestedBuilder = builder.AddNestedObject(nested.Key, nested.Value.Description, isRequired, nested.Value.IsArray);
                    AddNestedPropertiesToAnthropic(nestedBuilder, nested.Value);
                    nestedBuilder.EndNestedObject();
                }
                else
                {
                    builder.AddProperty(nested.Key, nested.Value.Type, nested.Value.Description, isRequired, nested.Value.Items);
                }
            }
        }

        #endregion

        #region OpenAI

        public static ToolOpenAI ToOpenAI(this UniversalToolDefinition definition)
        {
            var builder = new ToolTransformerBuilderOpenAI()
                .AddToolName(definition.Name)
                .AddDescription(definition.Description)
                .SetStrict(definition.Strict)
                .SetAdditionalProperties(definition.AdditionalProperties);

            ApplyMetadata(builder, definition);
            ApplyProperties(builder, definition);

            return builder.Build();
        }

        private static void ApplyMetadata(ToolTransformerBuilderOpenAI builder, UniversalToolDefinition definition)
        {
            if (definition.Keywords?.Count > 0)
                builder.AddKeyWords(definition.Keywords.ToArray());

            if (definition.Constraints?.Count > 0)
                builder.AddConstraint(definition.Constraints.ToArray());

            if (!string.IsNullOrEmpty(definition.InstructionHeader))
                builder.AddInstructionHeader(definition.InstructionHeader);

            if (definition.Instructions?.Count > 0)
            {
                foreach (var instruction in definition.Instructions)
                    builder.AddInstructions(instruction);
            }
        }

        private static void ApplyProperties(ToolTransformerBuilderOpenAI builder, UniversalToolDefinition definition)
        {
            foreach (var prop in definition.Properties)
            {
                AddPropertyToOpenAI(builder, prop.Key, prop.Value, definition.RequiredFields.Contains(prop.Key));
            }
        }

        private static void AddPropertyToOpenAI(ToolTransformerBuilderOpenAI builder, string name, UniversalProperty prop, bool isRequired)
        {
            if (prop.NestedProperties != null)
            {
                var nestedBuilder = builder.AddNestedObject(name, prop.Description, isRequired, prop.IsArray);
                AddNestedPropertiesToOpenAI(nestedBuilder, prop);
                nestedBuilder.EndObject();
            }
            else
            {
                builder.AddProperty(name, prop.Type, prop.Description, isRequired, prop.Items);
            }
        }

        private static void AddNestedPropertiesToOpenAI(NestedObjectBuilderOpenAI builder, UniversalProperty prop)
        {
            foreach (var nested in prop.NestedProperties)
            {
                var isRequired = prop.RequiredFields?.Contains(nested.Key) ?? false;

                if (nested.Value.NestedProperties != null)
                {
                    var nestedBuilder = builder.AddNestedObject(nested.Key, nested.Value.Description, isRequired, nested.Value.IsArray);
                    AddNestedPropertiesToOpenAI(nestedBuilder, nested.Value);
                    nestedBuilder.EndNestedObject();
                }
                else
                {
                    builder.AddProperty(nested.Key, nested.Value.Type, nested.Value.Description, isRequired, nested.Value.Items);
                }
            }
        }

        #endregion

        #region Gemini

        public static ToolGemini ToGemini(this UniversalToolDefinition definition)
        {
            var builder = new ToolTransformerBuilderGemini()
                .AddToolName(definition.Name)
                .AddDescription(definition.Description)
                .SetStrict(definition.Strict);

            ApplyMetadata(builder, definition);
            ApplyProperties(builder, definition);

            return builder.Build();
        }

        private static void ApplyMetadata(ToolTransformerBuilderGemini builder, UniversalToolDefinition definition)
        {
            if (definition.Keywords?.Count > 0)
                builder.AddKeyWords(definition.Keywords.ToArray());

            if (definition.Constraints?.Count > 0)
                builder.AddConstraint(definition.Constraints.ToArray());

            if (!string.IsNullOrEmpty(definition.InstructionHeader))
                builder.AddInstructionHeader(definition.InstructionHeader);

            if (definition.Instructions?.Count > 0)
            {
                foreach (var instruction in definition.Instructions)
                    builder.AddInstructions(instruction);
            }
        }

        private static void ApplyProperties(ToolTransformerBuilderGemini builder, UniversalToolDefinition definition)
        {
            foreach (var prop in definition.Properties)
            {
                AddPropertyToGemini(builder, prop.Key, prop.Value, definition.RequiredFields.Contains(prop.Key));
            }
        }

        private static void AddPropertyToGemini(ToolTransformerBuilderGemini builder, string name, UniversalProperty prop, bool isRequired)
        {
            if (prop.NestedProperties != null)
            {
                var nestedBuilder = builder.AddNestedObject(name, prop.Description, isRequired, prop.IsArray);
                AddNestedPropertiesToGemini(nestedBuilder, prop);
                nestedBuilder.EndObject();
            }
            else
            {
                builder.AddProperty(name, prop.Type, prop.Description, isRequired, prop.Items);
            }
        }

        private static void AddNestedPropertiesToGemini(NestedObjectBuilderGemini builder, UniversalProperty prop)
        {
            foreach (var nested in prop.NestedProperties)
            {
                var isRequired = prop.RequiredFields?.Contains(nested.Key) ?? false;

                if (nested.Value.NestedProperties != null)
                {
                    var nestedBuilder = builder.AddNestedObject(nested.Key, nested.Value.Description, isRequired, nested.Value.IsArray);
                    AddNestedPropertiesToGemini(nestedBuilder, nested.Value);
                    nestedBuilder.EndNestedObject();
                }
                else
                {
                    builder.AddProperty(nested.Key, nested.Value.Type, nested.Value.Description, isRequired, nested.Value.Items);
                }
            }
        }

        #endregion

        #region Groq

        public static GroqClient.ToolGroq ToGroq(this UniversalToolDefinition definition)
        {
            var builder = new ToolTransformerBuilderGroq()
                .AddToolName(definition.Name)
                .AddDescription(definition.Description)
                .SetStrict(definition.Strict)
                .SetAdditionalProperties(definition.AdditionalProperties);

            ApplyMetadata(builder, definition);
            ApplyProperties(builder, definition);

            return builder.Build();
        }

        private static void ApplyMetadata(ToolTransformerBuilderGroq builder, UniversalToolDefinition definition)
        {
            if (definition.Keywords?.Count > 0)
                builder.AddKeyWords(definition.Keywords.ToArray());

            if (definition.Constraints?.Count > 0)
                builder.AddConstraint(definition.Constraints.ToArray());

            if (!string.IsNullOrEmpty(definition.InstructionHeader))
                builder.AddInstructionHeader(definition.InstructionHeader);

            if (definition.Instructions?.Count > 0)
            {
                foreach (var instruction in definition.Instructions)
                    builder.AddInstructions(instruction);
            }
        }

        private static void ApplyProperties(ToolTransformerBuilderGroq builder, UniversalToolDefinition definition)
        {
            foreach (var prop in definition.Properties)
            {
                AddPropertyToGroq(builder, prop.Key, prop.Value, definition.RequiredFields.Contains(prop.Key));
            }
        }

        private static void AddPropertyToGroq(ToolTransformerBuilderGroq builder, string name, UniversalProperty prop, bool isRequired)
        {
            if (prop.NestedProperties != null)
            {
                var nestedBuilder = builder.AddNestedObject(name, prop.Description, isRequired, prop.IsArray);
                AddNestedPropertiesToGroq(nestedBuilder, prop);
                nestedBuilder.EndObject();
            }
            else
            {
                builder.AddProperty(name, prop.Type, prop.Description, isRequired, prop.Items);
            }
        }

        private static void AddNestedPropertiesToGroq(NestedObjectBuilderGroq builder, UniversalProperty prop)
        {
            foreach (var nested in prop.NestedProperties)
            {
                var isRequired = prop.RequiredFields?.Contains(nested.Key) ?? false;

                if (nested.Value.NestedProperties != null)
                {
                    var nestedBuilder = builder.AddNestedObject(nested.Key, nested.Value.Description, isRequired, nested.Value.IsArray);
                    AddNestedPropertiesToGroq(nestedBuilder, nested.Value);
                    nestedBuilder.EndNestedObject();
                }
                else
                {
                    builder.AddProperty(nested.Key, nested.Value.Type, nested.Value.Description, isRequired, nested.Value.Items);
                }
            }
        }

        #endregion

        #region LMStudio

        public static ToolLMStudio ToLMStudio(this UniversalToolDefinition definition)
        {
            var builder = new ToolTransformerBuilderLMStudio()
                .AddToolName(definition.Name)
                .AddDescription(definition.Description)
                .SetAdditionalProperties(definition.AdditionalProperties);

            ApplyMetadata(builder, definition);
            ApplyProperties(builder, definition);

            return builder.Build();
        }

        private static void ApplyMetadata(ToolTransformerBuilderLMStudio builder, UniversalToolDefinition definition)
        {
            if (definition.Keywords?.Count > 0)
                builder.AddKeyWords(definition.Keywords.ToArray());

            if (definition.Constraints?.Count > 0)
                builder.AddConstraint(definition.Constraints.ToArray());

            if (!string.IsNullOrEmpty(definition.InstructionHeader))
                builder.AddInstructionHeader(definition.InstructionHeader);

            if (definition.Instructions?.Count > 0)
            {
                foreach (var instruction in definition.Instructions)
                    builder.AddInstructions(instruction);
            }
        }

        private static void ApplyProperties(ToolTransformerBuilderLMStudio builder, UniversalToolDefinition definition)
        {
            foreach (var prop in definition.Properties)
            {
                AddPropertyToLMStudio(builder, prop.Key, prop.Value, definition.RequiredFields.Contains(prop.Key));
            }
        }

        private static void AddPropertyToLMStudio(ToolTransformerBuilderLMStudio builder, string name, UniversalProperty prop, bool isRequired)
        {
            if (prop.NestedProperties != null)
            {
                var nestedBuilder = builder.AddNestedObject(name, prop.Description, isRequired, prop.IsArray);
                AddNestedPropertiesToLMStudio(nestedBuilder, prop);
                nestedBuilder.EndObject();
            }
            else
            {
                builder.AddProperty(name, prop.Type, prop.Description, isRequired, prop.Items);
            }
        }

        private static void AddNestedPropertiesToLMStudio(NestedObjectBuilderLMStudio builder, UniversalProperty prop)
        {
            foreach (var nested in prop.NestedProperties)
            {
                var isRequired = prop.RequiredFields?.Contains(nested.Key) ?? false;

                if (nested.Value.NestedProperties != null)
                {
                    var nestedBuilder = builder.AddNestedObject(nested.Key, nested.Value.Description, isRequired, nested.Value.IsArray);
                    AddNestedPropertiesToLMStudio(nestedBuilder, nested.Value);
                    nestedBuilder.EndNestedObject();
                }
                else
                {
                    builder.AddProperty(nested.Key, nested.Value.Type, nested.Value.Description, isRequired, nested.Value.Items);
                }
            }
        }

        #endregion
    }
}
