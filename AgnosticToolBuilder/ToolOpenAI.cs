using AnthropicApp;

public class ToolOpenAI
{
    public string name { get; set; }
    public string description { get; set; }
    public bool strict { get; set; } = false;
    public ParametersOpenAI parameters { get; set; }

    public ToolOpenAI(string name, string description, ParametersOpenAI parameters, bool strict = false)
    {
        this.name = name;
        this.description = description;
        this.parameters = parameters;
        this.strict = strict;
    }
}

 
