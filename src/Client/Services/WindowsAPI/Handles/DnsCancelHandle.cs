using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Serilog;

namespace Ananke.Services.WindowsAPI.Handles;

public partial class DnsCancelHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    [LibraryImport("dnsapi.dll")]
    private static partial int DnsServiceRegisterCancel(
        IntPtr pCancel);

    public DnsCancelHandle() :
        base(ownsHandle: true)
    {
    }

    public DnsCancelHandle(
        IntPtr handle,
        bool ownsHandle = true) :
        base(ownsHandle)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        var ptr = Interlocked.Exchange(
            location1: ref handle,
            value: IntPtr.Zero);

        if (ptr == IntPtr.Zero)
            return true;

        var errorCode = DnsServiceRegisterCancel(ptr);

        if (errorCode == (int)WinSpecialCodes.ERROR_SUCCESS) 
            return true;
        
        Log.Error(
            messageTemplate: "Error while trying to cancel Dns service. Special code: {SpecialCode}.",
            propertyValue: errorCode);
        return false;
    }
}