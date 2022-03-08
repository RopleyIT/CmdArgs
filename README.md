# CmdArgs - Another Command Line Argument Parser
This simple library uses attributes on a property rich class to define the arguments that should be passed to the command.
Arguments determine their data type from the type of the property in the class, with attributes defining what the
command line format should be, and whether an argument is mandatory or optional.

## Usage

Consider two classes set up to map command line arguments to properties of those classes:

```
[ArgSet]
public class MyArgs
{
    [Arg("-t")]
    [Arg("--text")]
    public string? TextArg { get; set; }
    
    [Required]
    [Arg("-n")]
    public double Number { get; set; }
    
    [Arg("-y")]
    public bool TrueIfArgPresent { get; set; }
}

[ArgSet]
public class MoreArgs
{
    [Arg("-f")]
    public float Float { get; set; }
}
```
The presence of multiple Arg attributes on a property allows either format to be used. A boolean property is set
to true if the argument appears in the command line, false if not. The Required attribute will cause the argument parsing
to throw an Argument exception if the `-n` flag is not present in the example above.

To populate instances of these classes from a command line, in the `main` method use the following code:

```
static void main(string[] args)
{
    MyArgs myArgs = new();
    MoreArgs moreArgs = new();
    Arguments.Parse(args, myArgs, moreArgs);

    ... myArgs and moreArgs will have been populated with values ...
}
```

Examples of command lines that might parse correctly for the above class:

```
$ mycmd.exe --text "Text to go into the property TextArg" -n 3.14159
$ mycmd.exe -t "Short text flag" -n 1 -y -f 1.62E-19
```

## Command line help
The library will also automatically construct a multi-line usage string
for use with command line help messages. There is an additional `Description`
attribute that can be placed on the `ArgSet` class, and on each `Arg` property.
This allows you to automatically build the help string that describes each
group of options class by class, and within each class, describes each individual
set of arguments.

The classes below establish the documentation for each argument and argument set:

```
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
```

To generate the usage string for both these classes, use the `Arguments.Describe`
method as follows:

```
        MyArgs args = new();
        OtherArgs other = new();
        string description = Arguments.Describe(args, other);
```
A description for the above two classes might appear as follows:

```
The set of args
  -t "a string"  (optional)
    The string
  -i|--integer integer-value  (optional)
    The number
  -f float-value  (required)
    (no description)
  -s "a string"  (optional)
    (no description)
These are the other args
  -to "a string"  (optional)
    The other string
  -io|--other_integer integer-value  (optional)
    The other integer
  -fo float-value  (required)
    (no description)
  -so "a string"  (optional)
    (no description)
```