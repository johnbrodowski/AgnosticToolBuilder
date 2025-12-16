using System.Text;
 

namespace AnthropicApp 
{
 
    public class ParametersOpenAI
    {
        public string type { get; set; }
        public Dictionary<string, object> properties { get; set; }
        public bool additionalProperties { get; set; }
        public List<string> required { get; set; }

        public ParametersOpenAI(string type, Dictionary<string, object> properties, List<string> required = null, bool additionalProperties = false)
        {
            this.type = type;
            this.properties = properties;
            this.additionalProperties = additionalProperties;
            this.required = required;
        }
    }

    public class ToolTransformerBuilderOpenAI
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


        public ToolTransformerBuilderOpenAI AddToolName(string name)
        {
            _name = name ?? throw new ArgumentException("Tool name cannot be null.");
            return this;
        }

        public ToolTransformerBuilderOpenAI AddDescription(string description)
        {
            _description = description;
            return this;
        }

        public ToolTransformerBuilderOpenAI SetStrict(bool strict = false)
        {
            _strict = strict;
            return this;
        }

        public ToolTransformerBuilderOpenAI SetAdditionalProperties(bool allowAdditional)
        {
            _additionalProperties = allowAdditional;
            return this;
        }

        /// <summary>
        /// Adds constraints that limit when or how the tool should be used.
        /// </summary>
        /// <param name="constraints">One or more constraints to add</param>
        /// <returns>The builder instance for method chaining</returns>
        public ToolTransformerBuilderOpenAI AddConstraint(params string[] constraints)
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
        public ToolTransformerBuilderOpenAI AddKeyWords(params string[] keywords)
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
        public ToolTransformerBuilderOpenAI AddInstructionHeader(string instructionHeader)
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
        public ToolTransformerBuilderOpenAI AddInstructions(string instruction)
        {
            if (!string.IsNullOrEmpty(instruction))
            {
                _instructions.Add(instruction);
            }
            return this;
        }

 

        public NestedObjectBuilderOpenAI AddNestedObject(string objectName, string objectDescription, bool isRequired = true, bool isArray = false)
        {
            return new NestedObjectBuilderOpenAI(this, null, objectName, objectDescription, isRequired, isArray);
        }

        public ToolTransformerBuilderOpenAI AddProperty(
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

        public ToolOpenAI Build()
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

  
            var inputSchema = new ParametersOpenAI(
                type: _type,
                properties: _properties,
                additionalProperties: _additionalProperties,
                required: _requiredFields.Any() ? _requiredFields : null
            );

            return new ToolOpenAI(
                name: _name,
                description: descriptionBuilder.ToString()?.Trim() ?? "This tool processes input data and generates output.",
                parameters: inputSchema,
                strict: _strict
            );
  

        }
    }

    // NestedObjectBuilder remains largely the same, just updated to match new structure

    public class NestedObjectBuilderOpenAI
    {
        private readonly ToolTransformerBuilderOpenAI _parentBuilder;
        private readonly NestedObjectBuilderOpenAI _parentNestedBuilder;
        private readonly string _objectName;
        private readonly string _objectDescription;
        private readonly Dictionary<string, object> _properties = new();
        private readonly List<string> _requiredFields = new();
        private readonly bool _isArray;
        private readonly bool _isRequired;

        public NestedObjectBuilderOpenAI(
        ToolTransformerBuilderOpenAI parentBuilder,
        NestedObjectBuilderOpenAI parentNestedBuilder,
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


        public NestedObjectBuilderOpenAI AddNestedObject(string objectName, string objectDescription, bool isRequired = true, bool isArray = false)
        {
            return new NestedObjectBuilderOpenAI(null, this, objectName, objectDescription, isRequired, isArray);
        }

        public NestedObjectBuilderOpenAI AddProperty(
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

        public NestedObjectBuilderOpenAI EndNestedObject()
        {
            var definition = BuildDefinition();
            if (_parentNestedBuilder != null)
            {
                _parentNestedBuilder._properties[_objectName] = definition;
                return _parentNestedBuilder;
            }
            return this;
        }

        public ToolTransformerBuilderOpenAI EndObject()
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


    public static class ToolStringOutputOpenAI
    {
        public static string GenerateToolJson(ToolOpenAI tool)
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
 
 