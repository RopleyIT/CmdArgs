using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;

namespace CmdArgsTests
{

    [ArgSet]
    public class OtherArgs
    {
        [Arg("-to")]
        public string TArgOther { get; set; } = string.Empty;

        [Arg("-io")]
        [Arg("--other_integer")]
        public int? MyNumberOther { get; set; }

        [Arg("-fo"), Required]
        public float MyFloat { get; set; }

        [Arg("-so")]
        public string? TOptStringOther { get; set; }
    }
}
