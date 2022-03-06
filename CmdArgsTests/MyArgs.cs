using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;

namespace CmdArgsTests
{

    [ArgSet]
    public class MyArgs
    {
        [Arg("-t")]
        public string TArg { get; set; } = string.Empty;

        [Arg("-i")]
        [Arg("--integer")]
        public int? MyNumber { get; set; }

        [Arg("-f"), Required]
        public double MyDouble { get; set; }

        [Arg("-s")]
        public string? TOptString { get; set; }
    }
}
