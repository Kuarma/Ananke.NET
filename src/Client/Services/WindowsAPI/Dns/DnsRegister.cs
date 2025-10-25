using System.Runtime.InteropServices;

namespace Ananke.Services.WindowsAPI.Dns;

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
public delegate void DnsServiceRegisterComplete(
    uint status,
    IntPtr pQueryContext,
    IntPtr pInstanceName);

[StructLayout(LayoutKind.Sequential)]
public struct PDNS_SERVICE_REGISTER_REQUEST
{
    public uint Version;
    public uint InterfaceIndex;
    public IntPtr pServiceInstance;
    public IntPtr pRegisterCompletionCallback;
    public IntPtr pQueryContext;
    public IntPtr hCredentials;
    [MarshalAs(UnmanagedType.Bool)] public bool unicastEnabled;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct PDNS_SERVICE_INSTANCE
{
    [MarshalAs(UnmanagedType.LPWStr)] public string pszInstanceName;
    [MarshalAs(UnmanagedType.LPWStr)] public string pszHostName;
    public IntPtr ip4Address;
    public IntPtr ip6Address;
    public ushort wPort;
    public ushort wPriority;
    public ushort wWeight;
    public uint dwPropertyCount;
    public IntPtr keys;
    public IntPtr values;
    public uint dwInterfaceIndex;
}

[StructLayout(LayoutKind.Sequential)]
public struct PDNS_SERVICE_CANCEL
{
    public IntPtr reserved;
}