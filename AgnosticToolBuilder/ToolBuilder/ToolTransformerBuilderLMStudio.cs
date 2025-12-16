using System.Text;
using System.Text.Json;

/*




[
	{
		"type": "function",
		"function": {
			"name": "save_file",
			"Description": "Persists Editor content to the file system. This tool writes the current state of an Editor's content to a specified file location, ensuring work is preserved and accessible for subsequent operations.\n\nKeywords:\n- Data Persistence\n- File System Operations\n- Work Preservation\n\nConstraints:\n1. Verify file structure exists before saving\n2. Check for potential file conflicts or overwrites\n\nInstructions:\n# File Saving Guidelines #\n1. Validate file paths before saving to prevent errors\n2. Use consistent file naming patterns across the project",
			"parameters": {
				"type": "object",
				"properties": {
					"editor_config": {
						"type": "object",
						"Description": "Save operation parameters that specify the source Editor and target file location. Controls where and how Editor content is persisted.",
						"properties": {
							"editor_id": {
								"type": "string",
								"Description": "Unique identifier of the Editor instance containing the content to be saved. Must reference an active Editor in the current session."
							},
							"file_path": {
								"type": "string",
								"Description": "Full target path including directory structure and filename where the content will be written. Directories in the path must exist prior to saving."
							}
						},
						"required": [
							"editor_id",
							"file_path"
						]
					}
				},
				"additionalProperties": false,
				"required": [
					"editor_config"
				]
			}
		}
	},
	{
		"type": "function",
		"function": {
			"name": "open_file",
			"Description": "Loads existing file content into an Editor instance. This tool retrieves content from the file system and displays it in a specified Editor window, enabling review and modification of existing files.\n\nKeywords:\n- File Loading\n- Content Retrieval\n- Workspace Population\n\nConstraints:\n1. Target file must exist in the specified location\n2. Ensure appropriate Editor type for the file format\n\nInstructions:\n# File Opening Guidelines #\n1. Verify file existence before attempting to open\n2. Match Editor configuration to the file type being opened",
			"parameters": {
				"type": "object",
				"properties": {
					"editor_config": {
						"type": "object",
						"Description": "File loading parameters that specify the target Editor and source file. Controls how existing content is retrieved and displayed for editing.",
						"properties": {
							"editor_id": {
								"type": "string",
								"Description": "Unique identifier for the Editor instance where the file will be displayed. If the specified Editor doesn't exist, a new one will be created automatically."
							},
							"file_name": {
								"type": "string",
								"Description": "Base name of the file to load, including its extension. Used for display purposes and Editor configuration."
							},
							"file_path": {
								"type": "string",
								"Description": "Complete path to the source file, incorporating directory structure and filename. Must reference an existing file in the system."
							}
						},
						"required": [
							"editor_id",
							"file_name",
							"file_path"
						]
					}
				},
				"additionalProperties": false,
				"required": [
					"editor_config"
				]
			}
		}
	},
	{
		"type": "function",
		"function": {
			"name": "read_file",
			"Description": "Retrieves and displays file content from the file system. This tool accesses existing files and presents their contents for inspection, analysis, and reference without loading them into an Editor environment.\n\nKeywords:\n- Content Retrieval\n- File Inspection\n- Data Access\n\nConstraints:\n1. Target file must exist in the specified location\n2. Large files may impact performance and display\n\nInstructions:\n# File Reading Guidelines #\n1. Verify file existence before attempting to read\n2. Use for quick content inspection without editing needs\n3. Consider performance impact when reading very large files",
			"parameters": {
				"type": "object",
				"properties": {
					"file_class": {
						"type": "object",
						"Description": "File access parameters that specify the target content to retrieve. Controls which file is read and how its contents are presented.",
						"properties": {
							"file_path": {
								"type": "string",
								"Description": "Full path to the target file, including directory structure and filename. Must reference an existing accessible file in the project workspace."
							}
						},
						"required": [
							"file_path"
						]
					}
				},
				"additionalProperties": false,
				"required": [
					"file_class"
				]
			}
		}
	}
]





*/

namespace AnthropicApp
{
    public class ParametersLMStudio
    {
        public string type { get; set; }
        public Dictionary<string, object> properties { get; set; }
        public bool additionalProperties { get; set; }
        public List<string> required { get; set; }

        public ParametersLMStudio(string type, Dictionary<string, object> properties, List<string> required = null, bool additionalProperties = false)
        {
            this.type = type;
            this.properties = properties;
            this.additionalProperties = additionalProperties;
            this.required = required;
        }
    }

    public class FunctionLMStudio
    {
        public string name { get; set; }
        public string description { get; set; }
        public ParametersLMStudio parameters { get; set; }

        public FunctionLMStudio(string name, string description, ParametersLMStudio parameters)
        {
            this.name = name;
            this.description = description;
            this.parameters = parameters;
        }
    }

    public class ToolLMStudio
    {
        public string type { get; set; }
        public FunctionLMStudio function { get; set; }

        public ToolLMStudio(string name, string description, ParametersLMStudio parameters)
        {
            this.type = "function";
            this.function = new FunctionLMStudio(name, description, parameters);
        }
    }

    public class ToolTransformerBuilderLMStudio
    {
        private string _name;
        private string _description;
        private string _type = "object";
        private Dictionary<string, object> _properties = new();
        private List<string> _requiredFields = new();
        private bool _additionalProperties = false;
        private string _instructionHeader;
        private List<string> _keywords = new();
        private List<string> _constraints = new();
        private List<string> _instructions = new();

        public ToolTransformerBuilderLMStudio AddToolName(string name)
        {
            _name = name ?? throw new ArgumentException("Tool name cannot be null.");
            return this;
        }

        public ToolTransformerBuilderLMStudio AddDescription(string description)
        {
            _description = description;
            return this;
        }

        public ToolTransformerBuilderLMStudio SetAdditionalProperties(bool allowAdditional)
        {
            _additionalProperties = allowAdditional;
            return this;
        }

        /// <summary>
        /// Adds constraints that limit when or how the tool should be used.
        /// </summary>
        /// <param name="constraints">One or more constraints to add</param>
        /// <returns>The builder instance for method chaining</returns>
        public ToolTransformerBuilderLMStudio AddConstraint(params string[] constraints)
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
        public ToolTransformerBuilderLMStudio AddKeyWords(params string[] keywords)
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
        public ToolTransformerBuilderLMStudio AddInstructionHeader(string instructionHeader)
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
        public ToolTransformerBuilderLMStudio AddInstructions(string instruction)
        {
            if (!string.IsNullOrEmpty(instruction))
            {
                _instructions.Add(instruction);
            }
            return this;
        }

        public NestedObjectBuilderLMStudio AddNestedObject(string objectName, string objectDescription, bool isRequired = true, bool isArray = false)
        {
            return new NestedObjectBuilderLMStudio(this, null, objectName, objectDescription, isRequired, isArray);
        }

        public ToolTransformerBuilderLMStudio AddProperty(
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

        public ToolLMStudio Build()
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

            var inputSchema = new ParametersLMStudio(
                type: _type,
                properties: _properties,
                additionalProperties: _additionalProperties,
                required: _requiredFields.Any() ? _requiredFields : null
            );

            return new ToolLMStudio(
                name: _name,
                description: descriptionBuilder.ToString()?.Trim() ?? "This tool processes input data and generates output.",
                parameters: inputSchema
            );
        }
    }

    public class NestedObjectBuilderLMStudio
    {
        private readonly ToolTransformerBuilderLMStudio _parentBuilder;
        private readonly NestedObjectBuilderLMStudio _parentNestedBuilder;
        private readonly string _objectName;
        private readonly string _objectDescription;
        private readonly Dictionary<string, object> _properties = new();
        private readonly List<string> _requiredFields = new();
        private readonly bool _isArray;
        private readonly bool _isRequired;

        public NestedObjectBuilderLMStudio(
            ToolTransformerBuilderLMStudio parentBuilder,
            NestedObjectBuilderLMStudio parentNestedBuilder,
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

        public NestedObjectBuilderLMStudio AddNestedObject(string objectName, string objectDescription, bool isRequired = true, bool isArray = false)
        {
            return new NestedObjectBuilderLMStudio(null, this, objectName, objectDescription, isRequired, isArray);
        }

        public NestedObjectBuilderLMStudio AddProperty(
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

        public NestedObjectBuilderLMStudio EndNestedObject()
        {
            var definition = BuildDefinition();
            if (_parentNestedBuilder != null)
            {
                _parentNestedBuilder._properties[_objectName] = definition;
                return _parentNestedBuilder;
            }
            return this;
        }

        public ToolTransformerBuilderLMStudio EndObject()
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

    public static class ToolStringOutputLMStudio
    {
        public static string GenerateToolJson(ToolLMStudio tool)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(tool, options);
        }
    }





}
