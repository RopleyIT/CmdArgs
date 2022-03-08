using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;

namespace CmdArgsTests
{

    [ArgSet]
    [Description("These are the other args")]
    public class OtherArgs
    {
        [Description("The other string")]
        [Arg("-to")]
        public string TArgOther { get; set; } = string.Empty;

        [Description("The other integer")]
        [Arg("-io")]
        [Arg("--other_integer")]
        public int? MyNumberOther { get; set; }

        [Arg("-fo"), Required]
        public float MyFloat { get; set; }

        [Arg("-so")]
        public string? TOptStringOther { get; set; }
    }
}
