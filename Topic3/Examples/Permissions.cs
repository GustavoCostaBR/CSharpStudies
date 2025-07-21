namespace CSharpStudies.Topic3.Examples;

[Flags]
public enum Permissions
{
    None = 0,      // 0000
    Read = 1,      // 0001
    Write = 2,     // 0010
    Execute = 4,   // 0100
    All = Read | Write | Execute
}
