namespace CmdArgs
{
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ArgAttribute : System.Attribute
    {
        public string Tag { get; set; }
        public ArgAttribute(string tag) => Tag = tag;
    }
}