using System.Runtime.InteropServices;

namespace Ananke.Services.WindowsAPI;

// Use GC.KeepAlive to prevent GC from collecting the delegate.
[UnmanagedFunctionPointer(CallingConvention.Winapi)]
public delegate void DnsServiceRegisterComplete(
    uint status, 
    IntPtr pQueryContext, 
    DnsServiceInstance pInstanceName);

[StructLayout(LayoutKind.Sequential)]
public struct DnsServiceRegisterRequest
{
    public uint Version;
    public uint InterfaceIndex;
    public DnsServiceInstance pServiceInstance;
    public DnsServiceRegisterComplete pRegisterCompletionCallback;
    public IntPtr pQueryContext;
    public IntPtr hCredentials; 
    public bool unicastEnabled;
}

[StructLayout(LayoutKind.Sequential)]
public struct DnsServiceCancel
{ 
    public IntPtr reserved;
} 

[StructLayout(LayoutKind.Sequential)]
public struct DnsServiceInstance
{
    public string pszInstanceName;
    public IntPtr pszHostName;
    public uint ip4Address;
    public uint ip6Address;
    public ushort wPort;
    public ushort wPriority;
    public ushort wWeight;
    public uint dwPropertiesCount;
    public IntPtr keys;
    public IntPtr values;
    public uint dwInterfaceIndex;
}