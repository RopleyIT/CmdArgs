using System.Reflection;
using System.Text;

namespace CmdArgs
{
    /// <summary>
    /// Manage the parsing and documentation for command-line arguments
    /// passed into a program via the main(...) entrypoint method
    /// </summary>
    
    public static class Arguments
    {
        /// <summary>
        /// Parse the set of arguments passed on a comand line to
        /// a main function (entrypoint function) of an application
        /// </summary>
        /// <param name="args">The array of strings passed as comand
        /// line arguments (accessed as a span of strings)</param>
        /// <param name="argObjects">The array of ArgSet objects
        /// in order of parsing (arguments of one ArgSet must
        /// all be grouped together on the command line, before
        /// beginning the next set of arguments)</param>

        public static void Parse(Span<string> args, params object[] argObjects)
        {
            ValidateArgSetArguments(argObjects);

            foreach (object argObject in argObjects)
                args = ParseArgSet(args, argObject);
            if (args.Length > 0)
                throw new ArgumentException($"Unrecognised or duplicate argument: {args[0]}");
        }

        /// <summary>
        /// Produce a usage friendly description of each 
        /// set of arguments for documentation purposes
        /// </summary>
        /// <param name="argObjects">The ordered sequence
        /// of [ArgSet] objects</param>
        /// <returns>A descriptive string for use in
        /// run-time documentation</returns>

        public static string Describe(params object[] argObjects)
        {
            ValidateArgSetArguments(argObjects);

            StringBuilder description = new();
            foreach (object o in argObjects)
                DescribeArgProperties(description, o);
            return description.ToString();
        }

        private static void DescribeArgProperties(StringBuilder description, object o)
        {
            var descAttribute = o.GetType().GetCustomAttribute<DescriptionAttribute>();
            if (descAttribute != null)
                description.AppendLine(descAttribute.Text);

            PropertyInfo[] properties = o.GetType().GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                var argAttributes = pi.GetCustomAttributes<ArgAttribute>();
                descAttribute = pi.GetCustomAttribute<DescriptionAttribute>();
                var reqAttribute = pi.GetCustomAttribute<RequiredAttribute>();
                description.Append
                    ($"  {string.Join("|", argAttributes.Select(a => a.Tag))}");
                if (UnderlyingType(pi) == typeof(string))
                    description.Append(" \"a string\"");
                else if (UnderlyingType(pi) == typeof(int))
                    description.Append(" integer-value");
                else if (UnderlyingType(pi) == typeof(double)
                    || UnderlyingType(pi) == typeof(float))
                    description.Append(" float-value");
                if (reqAttribute != null)
                    description.Append("  (required)\r\n");
                else
                    description.Append("  (optional)\r\n");
                if (descAttribute != null)
                    description.Append($"    {descAttribute.Text}\r\n");
                else
                    description.Append("    (no description)\r\n");
            }
        }

        private static void ValidateArgSetArguments(object[] argSetList)
        {
            foreach(object o in argSetList)
                if (!HasArgSetAttribute(o))
                    throw new ArgumentException
                        ($"Type {o.GetType().Name} must have an [ArgSet] attribute");
        }

        private static Span<string> ParseArgSet(Span<string> args, object argObject)
        {
            var tagProperties = InitArgProperties(argObject);
            List<string> usedKeys = new();

            // Populate the dictionary with the tag
            // matchers and property instance references

            int i = 0;
            while (i < args.Length)
            {
                var tag = args[i];
                PropertyInstance pi;

                // First check that this argument has not
                // already been specified

                if (usedKeys.Contains(tag))
                    throw new ArgumentException($"Argument {tag} already used");

                if (tagProperties.ContainsKey(tag))
                    pi = tagProperties[tag];
                else
                    break;

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
                        && tagProperties[k].Instance == pi.Instance);
                usedKeys.AddRange(sharedKeys);
                foreach (var key in sharedKeys)
                    tagProperties.Remove(key);

                i++;
            }

            // Look for missing arguments that are required. The
            // only items that may still be in the tagProperties
            // collection at this point are optional ones.

            foreach (string key in tagProperties.Keys)
                if (RequiredArg(tagProperties[key].Info))
                    throw new ArgumentException
                        ($"Argument \"{key}\" missing or misplaced");

            // Truncate the span of arguments to those 
            // beyond the parsed part of the argument list

            return args[i..];
        }

        private static Dictionary<string, PropertyInstance> 
            InitArgProperties(object o)
        {
            Dictionary<string, PropertyInstance> tagProperties = new();
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
            return tagProperties;
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
