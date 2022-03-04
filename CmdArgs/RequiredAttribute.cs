using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdArgs
{
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredAttribute : System.Attribute { }
}
