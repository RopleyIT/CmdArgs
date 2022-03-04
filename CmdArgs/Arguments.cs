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
        public static T Parse(Span<string> args)
        {
            // Check that type T is a valid argument set

            if (!Arguments<T>.HasArgSetAttribute())
                throw new ArgumentException("Arguments<T>: T must have an [ArgSet] attribute");
            T t = new ();
            Dictionary<string, PropertyInfo> tagProperties = InitArgProperties();

            // Capture the argument values

            for(int i = 0; i < args.Length; i++)
            {
                var tag = args[i];
                PropertyInfo pi;
                if (tagProperties.ContainsKey(tag))
                    pi = tagProperties[tag];
                else
                    throw new ArgumentException($"Unrecognised or repeated argument: {tag}");

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
                            ($"Argument {tag} should be followed by a numeric value");
                    else if (Arguments<T>.UnderlyingType(pi) == typeof(int))
                    {
                        if (Math.Floor(value) != value)
                            throw new ArgumentException
                                ($"Argument {tag} should be followed by an integer");
                        pi.SetValue(t, (int)value);
                    }
                    else if (Arguments<T>.UnderlyingType(pi) == typeof(float))
                        pi.SetValue(t, (float)value);
                    else
                        pi.SetValue(t, value);
                }

                // Find all tags that refer to pi

                var sharedKeys = tagProperties.Keys
                    .Where(k => tagProperties[k] == pi)
                    .ToArray();
                foreach (var key in sharedKeys)
                    tagProperties.Remove(key);
            }

            // Look for missing arguments that are required. The
            // only items that may still be in the tagProperties
            // collection at this point are optional ones.

            foreach (string key in tagProperties.Keys)
                if (RequiredArg(tagProperties[key]))
                    throw new ArgumentException($"Argument \"{key}\" is missing");
            return t;
        }

        private static Type UnderlyingType(PropertyInfo pi)
            => Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

        private static Dictionary<string, PropertyInfo> InitArgProperties()
        {
            Dictionary<string, PropertyInfo> tagProperties = new();
            PropertyInfo[] properties = typeof(T).GetProperties();
            if (!properties.Any())
                throw new ArgumentException
                    ("Arguments<T>: Type T must have a property for each argument");
            foreach(PropertyInfo pi in properties)
            {
                var argAttributes = pi.GetCustomAttributes<ArgAttribute>();
                foreach (var argAttribute in argAttributes)
                    if (!string.IsNullOrWhiteSpace(argAttribute.Tag))
                        tagProperties.Add(argAttribute.Tag, pi);
                    else
                        throw new ArgumentException($"{pi.DeclaringType?.Name}.{pi.Name} "
                            + "has empty [Arg()] attribute");
            }
            return tagProperties;
        }

        private static bool RequiredArg(PropertyInfo pi) 
            => pi.GetCustomAttributes<RequiredAttribute>().Any();

        private static bool HasArgSetAttribute() 
            => typeof(T).GetCustomAttributes<ArgSetAttribute>().Any();
    }
}
