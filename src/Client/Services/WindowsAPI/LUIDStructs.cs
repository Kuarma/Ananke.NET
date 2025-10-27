using System.Runtime.InteropServices;

namespace Ananke.Services.WindowsAPI;

[StructLayout(LayoutKind.Sequential)]
public struct TOKEN_PRIVILEGES
{
    public uint PrivilegeCount;
    public LUID_ATTRIBUTES Privileges;
}

[StructLayout(LayoutKind.Sequential)]
public struct LUID_ATTRIBUTES
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