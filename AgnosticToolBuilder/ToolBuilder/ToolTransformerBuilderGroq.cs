using System.Text;

using Newtonsoft.Json;

namespace AnthropicApp
{
    // The local "ParametersGroq" class has been removed from this file to avoid
    // conflict with the main model class defined in "ParametersGroq.cs".
    // The builder will now use "AnthropicApp.GroqAI.ParametersGroq" directly.

    public class ToolTransformerBuilderGroq
    {
        private string _name;
        private string _description;
        private string _type = "object";
        private Dictionary<string, object> _properties = new();
        private List<string> _requiredFields = new();
        private bool _strict = false;
        private bool _additionalProperties = false;
        private string _instructionHeader;
        private List<string> _keywords = new();
        private List<string> _constraints = new();
        private List<string> _instructions = new();

        public ToolTransformerBuilderGroq AddToolName(string name)
        {
            _name = name ?? throw new ArgumentException("Tool name cannot be null.");
            return this;
        }

        public ToolTransformerBuilderGroq AddDescription(string description)
        {
            _description = description;
            return this;
        }

        public ToolTransformerBuilderGroq SetStrict(bool strict = false)
        {
            _strict = strict;
            return this;
        }

        public ToolTransformerBuilderGroq SetAdditionalProperties(bool allowAdditional)
        {
            _additionalProperties = allowAdditional;
            return this;
        }

        /// <summary>
        /// Adds constraints that limit when or how the tool should be used.
        /// </summary>
        /// <param name="constraints">One or more constraints to add</param>
        /// <returns>The builder instance for method chaining</returns>
        public ToolTransformerBuilderGroq AddConstraint(params string[] constraints)
        {
            if (constraints != null)
            {
                foreach (var constraint in constraints)
                {
                    if (!string.IsNullOrEmpty(constraint))
                    {
                        _constraints.Add(constraint);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Adds keywords that help categorize or identify the tool's purpose.
        /// </summary>
        /// <param name="keywords">One or more keywords to add</param>
        /// <returns>The builder instance for method chaining</returns>
        public ToolTransformerBuilderGroq AddKeyWords(params string[] keywords)
        {
            if (keywords != null)
            {
                foreach (var keyword in keywords)
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        _keywords.Add(keyword);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Sets the header for the instructions section.
        /// </summary>
        /// <param name="instructionHeader">The header text for instructions</param>
        /// <returns>The builder instance for method chaining</returns>
        public ToolTransformerBuilderGroq AddInstructionHeader(string instructionHeader)
        {
            if (!string.IsNullOrEmpty(instructionHeader))
            {
                _instructionHeader = instructionHeader;
            }
            return this;
        }

        /// <summary>
        /// Adds a specific instruction for using the tool.
        /// </summary>
        /// <param name="instruction">The instruction to add</param>
        /// <returns>The builder instance for method chaining</returns>
        public ToolTransformerBuilderGroq AddInstructions(string instruction)
        {
            if (!string.IsNullOrEmpty(instruction))
            {
                _instructions.Add(instruction);
            }
            return this;
        }

        public NestedObjectBuilderGroq AddNestedObject(string objectName, string objectDescription, bool isRequired = true, bool isArray = false)
        {
            return new NestedObjectBuilderGroq(this, null, objectName, objectDescription, isRequired, isArray);
        }

        public ToolTransformerBuilderGroq AddProperty(
            string fieldName,
            string fieldType,
            string fieldDescription,
            bool isRequired = false,
            Dictionary<string, string> items = null)
        {
            var propertyDef = new Dictionary<string, object>
            {
                { "type", fieldType },
                { "description", fieldDescription }
            };

            if (items != null)
            {
                propertyDef["items"] = items;
            }

            _properties[fieldName] = propertyDef;

            if (isRequired)
            {
                _requiredFields.Add(fieldName);
            }

            return this;
        }

        internal void SetNestedObject(string objectName, Dictionary<string, object> properties, bool isRequired, bool isArray)
        {
            var props = properties["properties"] as Dictionary<string, object>;
            bool isSingleProperty = props?.Count == 1;

            if (isArray)
            {
                var arraySchema = new Dictionary<string, object>
                {
                    { "type", "array" },
                    { "items", properties }
                };
                _properties[objectName] = arraySchema;
            }
            else
            {
                _properties[objectName] = properties;
            }

            if (isRequired)
            {
                _requiredFields.Add(objectName);
            }
        }

        public GroqClient.ToolGroq Build()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new InvalidOperationException("Tool name must be set before building.");
            }

            var descriptionBuilder = new StringBuilder();
            descriptionBuilder.Append(_description?.Trim() ?? "This tool processes input data and generates output.");

            // Format and add keywords section
            if (_keywords.Count > 0)
            {
                descriptionBuilder.Append("\n\nKeywords:");
                foreach (var keyword in _keywords)
                {
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        descriptionBuilder.Append($"\n- {keyword}");
                    }
                }
            }

            // Format and add constraints section
            if (_constraints.Count > 0)
            {
                descriptionBuilder.Append("\n\nConstraints:");
                int count = 1;

                foreach (var constraint in _constraints)
                {
                    if (!string.IsNullOrWhiteSpace(constraint))
                    {
                        descriptionBuilder.Append($"\n{count}. {constraint}");
                        count++;
                    }
                }
            }

            // Format and add instructions section
            if (_instructions.Count > 0)
            {
                descriptionBuilder.Append("\n\nInstructions:");
                int count = 1;

                if (!string.IsNullOrWhiteSpace(_instructionHeader))
                {
                    descriptionBuilder.Append($"\n# {_instructionHeader} #");
                }

                foreach (var instruction in _instructions)
                {
                    if (!string.IsNullOrWhiteSpace(instruction))
                    {
                        descriptionBuilder.Append($"\n{count}. {instruction}");
                        count++;
                    }
                }
            }

            // Create the parameters schema using the correct model class
            var inputSchema = new GroqClient.ParametersGroq
            {
                Type = _type,
                Properties = _properties,
                Required = _requiredFields.ToArray()
            };
            // Note: _additionalProperties is ignored as it's not in the target model.

            // Create the function definition that holds the tool's details
            var functionDefinition = new GroqClient.FunctionDefinitionGroq
            {
                Name = _name,
                Description = descriptionBuilder.ToString()?.Trim() ?? "This tool processes input data and generates output.",
                Parameters = inputSchema
            };

            // Create the final ToolGroq object with the correct nested structure.
            // Note: _strict is ignored as it's not in the target model.
            return new GroqClient.ToolGroq
            {
                Function = functionDefinition
            };
        }
    }

    // NestedObjectBuilder remains largely the same, just updated to match new structure
    public class NestedObjectBuilderGroq
    {
        private readonly ToolTransformerBuilderGroq _parentBuilder;
        private readonly NestedObjectBuilderGroq _parentNestedBuilder;
        private readonly string _objectName;
        private readonly string _objectDescription;
        private readonly Dictionary<string, object> _properties = new();
        private readonly List<string> _requiredFields = new();
        private readonly bool _isArray;
        private readonly bool _isRequired;

        public NestedObjectBuilderGroq(
        ToolTransformerBuilderGroq parentBuilder,
        NestedObjectBuilderGroq parentNestedBuilder,
        string objectName,
        string objectDescription,
        bool isRequired,
        bool isArray)
        {
            _parentBuilder = parentBuilder;
            _parentNestedBuilder = parentNestedBuilder;
            _objectName = objectName;
            _objectDescription = objectDescription;
            _isArray = isArray;
            _isRequired = isRequired;
        }

        public NestedObjectBuilderGroq AddNestedObject(string objectName, string objectDescription, bool isRequired = true, bool isArray = false)
        {
            return new NestedObjectBuilderGroq(null, this, objectName, objectDescription, isRequired, isArray);
        }

        public NestedObjectBuilderGroq AddProperty(
            string fieldName,
            string fieldType,
            string fieldDescription,
            bool isRequired = false,
            Dictionary<string, string> items = null)
        {
            var propertyDef = new Dictionary<string, object>
            {
                { "type", fieldType },
                { "description", fieldDescription }
            };

            if (items != null)
            {
                propertyDef["items"] = items;
            }

            _properties[fieldName] = propertyDef;

            if (isRequired)
            {
                _requiredFields.Add(fieldName);  // Add to this object's required fields
            }

            return this;
        }

        // In NestedObjectBuilder
        private Dictionary<string, object> BuildDefinition()
        {
            var objectDefinition = new Dictionary<string, object>
            {
                { "type", "object" },
                { "description", _objectDescription },
                { "properties", _properties }
            };

            if (_requiredFields.Count > 0)
            {
                objectDefinition["required"] = _requiredFields;
            }

            return objectDefinition;
        }

        public NestedObjectBuilderGroq EndNestedObject()
        {
            var definition = BuildDefinition();
            if (_parentNestedBuilder != null)
            {
                _parentNestedBuilder._properties[_objectName] = definition;
                return _parentNestedBuilder;
            }
            return this;
        }

        public ToolTransformerBuilderGroq EndObject()
        {
            if (_parentBuilder == null)
            {
                throw new InvalidOperationException("Cannot end object without a parent builder");
            }

            var objectDefinition = BuildDefinition();
            _parentBuilder.SetNestedObject(_objectName, objectDefinition, _isRequired, _isArray);
            return _parentBuilder;
        }
    }

    public static class ToolStringOutputGroq
    {
        public static string GenerateToolJson(GroqClient.ToolGroq tool)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(tool, settings);
        }
    }
}