namespace Ananke.Services.WindowsAPI;

[Flags]
public enum Privileges : uint
{
    TokenAdjustPrivileges = 0x20,
    TokenQuery = 0x8
}