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
