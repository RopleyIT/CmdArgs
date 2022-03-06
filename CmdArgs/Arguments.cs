using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CmdArgs
{
    public static class Arguments
    {
        public static void Parse(Span<string> args, params object[] argObjects)
        {
            Dictionary<string, PropertyInstance> tagProperties = new();

            // Check that each argument object is a valid argument set

            foreach(object o in argObjects)
                if (!HasArgSetAttribute(o))
                    throw new ArgumentException
                        ($"Type {o.GetType().Name} must have an [ArgSet] attribute");
                else
                    InitArgProperties(tagProperties, o);

            // Capture the argument values

            for(int i = 0; i < args.Length; i++)
            {
                var tag = args[i];
                PropertyInstance pi;
                if (tagProperties.ContainsKey(tag))
                    pi = tagProperties[tag];
                else
                    throw new ArgumentException($"Unrecognised or repeated argument: {tag}");

                if (UnderlyingType(pi.Info) == typeof(bool))
                    pi.Info.SetValue(pi.Instance, true);
                else if (UnderlyingType(pi.Info) == typeof(string))
                    pi.Info.SetValue(pi.Instance, args[++i]);
                else if (UnderlyingType(pi.Info) == typeof(int) 
                    || UnderlyingType(pi.Info) == typeof(double)
                    || UnderlyingType(pi.Info) == typeof(float))
                {
                    if (!double.TryParse(args[++i], out double value))
                        throw new ArgumentException
                            ($"Argument {tag} should be followed by a numeric value");
                    else if (UnderlyingType(pi.Info) == typeof(int))
                    {
                        if (Math.Floor(value) != value)
                            throw new ArgumentException
                                ($"Argument {tag} should be followed by an integer");
                        pi.Info.SetValue(pi.Instance, (int)value);
                    }
                    else if (UnderlyingType(pi.Info) == typeof(float))
                        pi.Info.SetValue(pi.Instance, (float)value);
                    else
                        pi.Info.SetValue(pi.Instance, value);
                }

                // Find all tags that refer to pi

                var sharedKeys = tagProperties.Keys
                    .Where(k => tagProperties[k].Info == pi.Info
                        && tagProperties[k].Instance == pi.Instance)
                    .ToArray();
                foreach (var key in sharedKeys)
                    tagProperties.Remove(key);
            }

            // Look for missing arguments that are required. The
            // only items that may still be in the tagProperties
            // collection at this point are optional ones.

            foreach (string key in tagProperties.Keys)
                if (RequiredArg(tagProperties[key].Info))
                    throw new ArgumentException($"Argument \"{key}\" is missing");
        }

        private static void InitArgProperties
            (Dictionary<string, PropertyInstance> tagProperties, object o)
        {
            PropertyInfo[] properties = o.GetType().GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                var argAttributes = pi.GetCustomAttributes<ArgAttribute>();
                foreach (var argAttribute in argAttributes)
                    if (!string.IsNullOrWhiteSpace(argAttribute.Tag))
                        tagProperties.Add(argAttribute.Tag, new PropertyInstance(o, pi));
                    else
                        throw new ArgumentException($"{pi.DeclaringType?.Name}.{pi.Name} "
                            + "has empty [Arg()] attribute");
            }
        }

        private static Type UnderlyingType(PropertyInfo pi)
            => Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

        private static bool RequiredArg(PropertyInfo pi) 
            => pi.GetCustomAttributes<RequiredAttribute>().Any();

        private static bool HasArgSetAttribute(object o)
        {
            Type t = o.GetType();
            if (!t.GetCustomAttributes<ArgSetAttribute>().Any())
                return false;
            if (!t.IsClass)
                return false;
            return true;
        }
    }
}
