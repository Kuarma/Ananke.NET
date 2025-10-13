namespace Ananke.WindowsAPI;

[Flags]
public enum Privileges : uint
{
    TokenAdjustPrivileges = 0x020,
    TokenQuery = 0x8
}