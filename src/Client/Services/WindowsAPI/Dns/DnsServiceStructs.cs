using System.Runtime.InteropServices;

namespace Ananke.Services.WindowsAPI.Dns;

[StructLayout(LayoutKind.Sequential)]
public struct DNS_SERVICE_BROWSE_REQUEST
{
}

[StructLayout(LayoutKind.Sequential)]
public struct DNS_SERVICE_CANCEL
{
    public IntPtr reserved;
}