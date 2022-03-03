using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CmdArgs
{
    public static class Arguments<T> where T : class, new()
    {
        public static T Parse(string[] args, T? t = null)
        {
            // Check that type T is a valid argument set

            if (!Arguments<T>.HasArgSetAttribute())
                throw new ArgumentException("Arguments<T>: T must have an [ArgSet] attribute");
            if(t == null)
                t = new T();
            Dictionary<string, PropertyInfo?> tagProperties = InitArgProperties();

            // Capture the argument values

            for(int i = 0; i < args.Length; i++)
            {
                PropertyInfo? pi = tagProperties.GetValueOrDefault(args[i], null);
                if (pi == null)
                    throw new ArgumentException($"Unrecognised argument: {args[i]}");

                if (Arguments<T>.UnderlyingType(pi) == typeof(bool))
                    pi.SetValue(t, true);
                else if (Arguments<T>.UnderlyingType(pi) == typeof(string))
                    pi.SetValue(t, args[++i]);
                else if (Arguments<T>.UnderlyingType(pi) == typeof(int) 
                    || Arguments<T>.UnderlyingType(pi) == typeof(double)
                    || Arguments<T>.UnderlyingType(pi) == typeof(float))
                {
                    if (!double.TryParse(args[++i], out double value))
                        throw new ArgumentException
                            ($"Argument {args[i - 1]} should be followed by a numeric value");
                    else if (Arguments<T>.UnderlyingType(pi) == typeof(int))
                    {
                        if (Math.Floor(value) != value)
                            throw new ArgumentException
                                ($"Argument {args[i - 1]} should be followed by an integer");
                        pi.SetValue(t, (int)value);
                    }
                    else if (Arguments<T>.UnderlyingType(pi) == typeof(float))
                        pi.SetValue(t, (float)value);
                    else
                        pi.SetValue(t, value);
                }
            }
            return t;
        }

        private static Type UnderlyingType(PropertyInfo pi)
            => Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

        private static Dictionary<string, PropertyInfo?> InitArgProperties()
        {
            Dictionary<string, PropertyInfo?> tagProperties = new();
            var properties = typeof(T).GetProperties();
            if (properties == null || properties.Length == 0)
                throw new ArgumentException("Arguments<T>: T must have properties to capture arguments");
            foreach(var pi in properties)
            {
                var argAttributes = pi.GetCustomAttributes<ArgAttribute>();
                foreach(var argAttribute in argAttributes)
                    if(!string.IsNullOrWhiteSpace(argAttribute.Tag))
                        tagProperties.Add(argAttribute.Tag, pi);
            }
            return tagProperties;
        }

        private static bool HasArgSetAttribute()
        {
            var attributes = typeof(T).GetCustomAttributes(typeof (ArgSetAttribute), false);
            if (attributes == null || attributes.Length == 0)
                return false;
            if (attributes[0] as ArgSetAttribute == null)
                return false;
            return true;
        }
    }
}
