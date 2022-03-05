# CmdArgs - Another Command Line Argument Parser
This simple library uses attributes on a property rich class to define the arguments that should be passed to the command.
Arguments determine their data type from the type of the property in the class, with attributes defining what the
command line format should be, and whether an argument is mandatory or optional.

## Usage
```
[Argset]
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
```
The presence of multiple Arg attributes on a property allows either format to be used. A boolean property is set
to true of the argument is included, false if omitted. The Required attribute will cause the argument parsing
to throw an Argument exception if th `-n` flag in the eample above is not present.

Examples of command lines that might parse correctly for the above class:

```
$ mycmd.exe --text "Text to go into the property TextArg" -n 3.14159
$ mycmd.exe -t "Short text flag" -n 1 -y
```
