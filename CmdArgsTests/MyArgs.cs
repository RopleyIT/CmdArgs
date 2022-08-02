using CmdArgs;

namespace CmdArgsTests
{

    [ArgSet]
    [Description("The set of args")]
    public class MyArgs
    {
        [Description("The string")]
        [Arg("-t")]
        public string TArg { get; set; } = string.Empty;

        [Description("The number")]
        [Arg("-i")]
        [Arg("--integer")]
        public int? MyNumber { get; set; }

        [Arg("-f"), Required]
        public double MyDouble { get; set; }

        [Arg("-s")]
        public string? TOptString { get; set; }
    }
}
