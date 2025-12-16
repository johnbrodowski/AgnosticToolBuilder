namespace AnthropicApp
{
    public class ToolGemini
    {
        public string name { get; set; }
        public string description { get; set; }
        public bool strict { get; set; } = false;
        public ParametersGemini parameters { get; set; }
        public ToolGemini(string _name, string _description, ParametersGemini _parameters, bool _strict = false)
        {
            this.name = _name; this.description = _description; this.parameters = _parameters; this.strict = _strict;
        }
    }

}
 