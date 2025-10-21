using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Ananke.Extensions;
using Microsoft.Extensions.Logging;

namespace Ananke.Services.WindowsAPI;

// https://github.com/dotnet/roslyn-analyzers/issues/7690
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public partial class ProcessTokenManager : IProcessTokenManager
{
    [LibraryImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AdjustTokenPrivileges(
        SafeMemoryHandle safeMemoryHandle,
        [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges,
        ref TokenPrivileges newState,
        uint bufferLength,
        IntPtr previousState,
        IntPtr returnLength);

    [LibraryImport("advapi32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool LookupPrivilegeValueW(
        string lpSystemName,
        string lpName,
        out LUID lpLUID);

    [LibraryImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool OpenProcessToken(
        IntPtr processHandle,
        Privileges desiredAccess,
        out SafeMemoryHandle safeMemoryHandle);

    private readonly ILogger<ProcessTokenManager> _logger;
    private readonly SafeMemoryHandle _safeMemoryHandle;

    public ProcessTokenManager(
        ILogger<ProcessTokenManager> logger)
    {
        _logger = logger;

        if (!OpenProcessToken(
                processHandle: Process.GetCurrentProcess().Handle,
                desiredAccess: Privileges.TokenAdjustPrivileges | Privileges.TokenQuery,
                safeMemoryHandle: out _safeMemoryHandle))
        {
            _logger.LogError("Failed to open process token. Error code: {ErrorCode}",
                Marshal.GetLastWin32Error());
        }
    }

    public bool TrySetProcessPrivilege(
        string privilegeName = SEPrivileges.SE_SHUTDOWN_NAME,
        uint privilegeCount = SEPrivileges.SE_PRIVILEGE_COUNT,
        uint privilegeAttributes = SEPrivileges.SE_PRIVILEGE_ENABLED)
    {
        var tokenPrivileges = new TokenPrivileges
        {
            PrivilegeCount = privilegeCount,
            Privileges = new LUIDAttributes
            {
                Attributes = privilegeAttributes
            }
        };

        if (!LookupPrivilegeValueW(
                lpSystemName: "",
                lpName: privilegeName,
                lpLUID: out tokenPrivileges.Privileges.LUID))
        {
            _logger.LogError(
                "Failed to lookup Privilege Value. Error code: {ErrorCode}",
                Marshal.GetLastWin32Error());
            return false;
        }

        if (AdjustTokenPrivileges(
                safeMemoryHandle: _safeMemoryHandle,
                disableAllPrivileges: false,
                newState: ref tokenPrivileges,
                bufferLength: 0U,
                previousState: IntPtr.Zero,
                returnLength: IntPtr.Zero))
            return true;

        _logger.LogError(
            "Failed to adjust token privileges. Error code: {ErrorCode}",
            Marshal.GetLastWin32Error());
        return false;
    }
    
    public bool TryDisableHandle()
    {
        TokenPrivileges tokenPrivileges = default;

        if (!AdjustTokenPrivileges(
                safeMemoryHandle: _safeMemoryHandle,
                disableAllPrivileges: true,
                newState: ref tokenPrivileges,
                bufferLength: 0U,
                previousState: IntPtr.Zero,
                returnLength: IntPtr.Zero))
        {
            _logger.LogError("Failed to disable handle. Error code: {ErrorCode}",
                Marshal.GetLastWin32Error());
            return false;
        }

        _safeMemoryHandle.Close(); 
        
        _logger.LogInformation("Token handle successfully disabled and disposed.");
        return true;
    }
}