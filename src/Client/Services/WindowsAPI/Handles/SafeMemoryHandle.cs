using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Ananke.Services.WindowsAPI.Handles;

// https://essentialcsharp.com/using-safehandle
public sealed partial class SafeMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(
        IntPtr hObject);

    public SafeMemoryHandle() :
        base(
            ownsHandle: true)
    {
    }

    protected override bool ReleaseHandle()
    {
        return CloseHandle(handle);
    }
}