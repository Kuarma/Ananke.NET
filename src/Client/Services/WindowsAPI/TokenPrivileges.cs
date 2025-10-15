using System.Runtime.InteropServices;

namespace Ananke.Services.WindowsAPI;

[StructLayout(LayoutKind.Sequential)]
public struct TokenPrivileges
{
    public int PrivilegeCount;
    public LUIDAttributes Privileges;
}

[StructLayout(LayoutKind.Sequential)]
public struct LUIDAttributes
{
    public LUID LUID;
    public uint Attributes;
}

[StructLayout(LayoutKind.Sequential)]
public struct LUID
{
    public uint LowPart;
    public int HighPart;
}