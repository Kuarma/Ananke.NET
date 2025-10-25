using System.Runtime.InteropServices;
using Serilog;

namespace Ananke.Services.WindowsAPI.Handles;

public partial class DnsCancelHandle : SafeHandle
{
    [LibraryImport("dnsapi.dll", SetLastError = true)]
    private static partial int DnsServiceRegisterCancel(IntPtr pCancel);

    public DnsCancelHandle() :
        base(IntPtr.Zero, true)
    {
        handle = Marshal.AllocHGlobal(8);
    }

    public override bool IsInvalid =>
        handle == IntPtr.Zero;

    internal IntPtr GetInternalValue()
    {
        if (handle != IntPtr.Zero)
            return Marshal.ReadIntPtr(handle);

        return IntPtr.Zero;
    }

    public void Clear()
    {
        if (handle == IntPtr.Zero)
            return;

        Marshal.FreeHGlobal(handle);
        handle = IntPtr.Zero;
    }

    protected override bool ReleaseHandle()
    {
        if (handle == IntPtr.Zero)
            return true;

        var errorCode = DnsServiceRegisterCancel(handle);

        if (errorCode != 0)
        {
            Log.Error("Failed to cancel DNS service. Error code: {ErrorCode}. " +
                      "Check for code here: https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-",
                errorCode);

            return false;
        }

        Marshal.FreeHGlobal(handle);
        handle = IntPtr.Zero;

        return true;
    }
}