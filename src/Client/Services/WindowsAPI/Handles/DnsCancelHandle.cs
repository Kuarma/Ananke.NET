using System.Runtime.InteropServices;
using Serilog;

namespace Ananke.Services.WindowsAPI.Handles;

public partial class DnsCancelHandle : SafeHandle
{
    [LibraryImport("dnsapi.dll", SetLastError = true)]
    private static partial int DnsServiceRegisterCancel(
        IntPtr pCancel);

    public DnsCancelHandle() :
        base(
            invalidHandleValue: IntPtr.Zero, 
            ownsHandle: true)
    {
        handle = Marshal.AllocHGlobal(8);
    }

    public override bool IsInvalid =>
        handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (handle == IntPtr.Zero)
            return true;

        var errorCode = DnsServiceRegisterCancel(handle);

        try
        {
            Marshal.FreeHGlobal(handle);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to free DNS service handle.");
        }
        finally
        {
            handle = IntPtr.Zero; // Ensure the handle is set to null to prevent double free.
        }

        if (errorCode == (int)WindowsErrorCodes.ERROR_SUCCESS) 
            return true;
        
        Log.Error(
            "Failed to cancel DNS service. Error code: {ErrorCode}",
            errorCode);
        
        return false;
    }
}