namespace CmdArgs
{
    [System.AttributeUsage(AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Property, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Text { get; init; }
        public DescriptionAttribute(string text) => Text = text;
    }
}
