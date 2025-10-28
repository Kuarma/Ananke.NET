using System.Runtime.InteropServices;
using Serilog;

namespace Ananke.Services.WindowsAPI.Handles;

public partial class DnsHandle : SafeHandle
{
    [LibraryImport("dnsapi.dll")]
    private static partial int DnsServiceRegisterCancel(
        IntPtr pCancel);
    
    public DnsHandle() :
        base(
            invalidHandleValue: IntPtr.Zero,
            ownsHandle: true)
    {
    }

    public override bool IsInvalid =>
        handle == IntPtr.Zero;
    
    protected override bool ReleaseHandle()
    {
        var ptr = Interlocked.Exchange(
            location1: ref handle,
            value: IntPtr.Zero);

        if (ptr == IntPtr.Zero)
            return true;
        
        var errorCode = DnsServiceRegisterCancel(ptr);
        
        if (errorCode != (int)WinSpecialCodes.ERROR_SUCCESS)
            Log.Error(
                messageTemplate: "Error while trying to cancel Dns service. Special code: {SpecialCode}.",
                propertyValue: errorCode);

        return true;
    }
}