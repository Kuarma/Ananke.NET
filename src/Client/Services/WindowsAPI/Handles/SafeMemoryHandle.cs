using System.Runtime.InteropServices;

namespace Ananke.Services.WindowsAPI.Handles;

// https://essentialcsharp.com/using-safehandle
public sealed partial class SafeMemoryHandle : SafeHandle
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(
        IntPtr hObject);

    public SafeMemoryHandle() :
        base(
            invalidHandleValue: IntPtr.Zero,
            ownsHandle: true)
    {
    }

    public override bool IsInvalid =>
        handle == IntPtr.Zero; 

    protected override bool ReleaseHandle()
    {
        return CloseHandle(handle);
    }
}