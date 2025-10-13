using System.Runtime.InteropServices;

namespace Ananke.WindowsAPI;

[StructLayout(LayoutKind.Sequential)]
public struct TokenAttributes
{
    public int PrivilegeCount;
    public LUIDAttributes Privileges;
}

[StructLayout(LayoutKind.Sequential)]
public struct LUIDAttributes
{
    public LUID Luid;
    public uint Attributes;
}

[StructLayout(LayoutKind.Sequential)]
public struct LUID
{
    public uint LowPart;
    public int HighPart;
}
