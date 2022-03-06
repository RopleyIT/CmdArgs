using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CmdArgs
{
    internal class PropertyInstance
    {
        public PropertyInstance(object argObject, PropertyInfo propertyInfo)
        {
            Info = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            Instance = argObject ?? throw new ArgumentNullException(nameof(argObject));
        }

        public PropertyInfo Info { get; init; }
        public object Instance { get; init; }
    }
}
