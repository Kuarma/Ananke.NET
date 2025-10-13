using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Ananke.WindowsAPI;

public partial class ProcessTokenManager
{
    [LibraryImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AdjustTokenPrivileges(
        IntPtr tokenPrivileges,
        [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges,
        ref TokenAttributes newState,
        uint bufferLength,
        IntPtr previousState,
        IntPtr returnLength);

    [LibraryImport("advapi32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool LookupPrivilegeValue(
        string lpSystemName,
        string lpName,
        out LUID lpLuid);
    
    [LibraryImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool OpenProcessToken(
        IntPtr processHandle,
        Privileges desiredAccess,
        out IntPtr tokenHandle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CloseHandle(
        IntPtr hObject);

    private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
    private const int SE_PRIVILEGE_ENABLED = 2;
    private const int SE_PRIVILEGE_COUNT = 1;

    private readonly ILogger<ProcessTokenManager> _logger;

    public ProcessTokenManager(
        ILogger<ProcessTokenManager> logger)
    {
        _logger = logger;
    }

    public void EnableShutdownPrivilege()
    {
        TokenAttributes tokenAttributes;
        tokenAttributes.PrivilegeCount = SE_PRIVILEGE_COUNT;
        tokenAttributes.Privileges.Attributes = SE_PRIVILEGE_ENABLED;

        if (!LookupPrivilegeValue(
                lpSystemName: "",
                lpName: SE_SHUTDOWN_NAME,
                lpLuid: out tokenAttributes.Privileges.Luid))
        {
            _logger.LogError(
                "Failed to lookup Privilege Value: {tokenPrivileges}",
                tokenAttributes);
            throw new Win32Exception();
        }

        AdjustPrivileges(
            disableAllPrivileges: false,
            tokenHandle: GetProcessToken(),
            tokenAttributes: tokenAttributes);
    }

    public void AdjustPrivileges(
        bool disableAllPrivileges,
        IntPtr tokenHandle,
        TokenAttributes tokenAttributes = default)
    {
        if (!AdjustTokenPrivileges(
                tokenPrivileges: tokenHandle,
                disableAllPrivileges: disableAllPrivileges,
                newState: ref tokenAttributes,
                bufferLength: 0U,
                previousState: IntPtr.Zero,
                returnLength: IntPtr.Zero))
            _logger.LogError("Failed to adjust token privileges: {tokenHandle}",
                tokenHandle);
    }

    public IntPtr GetProcessToken()
    {
        if (OpenProcessToken(
                processHandle: Process.GetCurrentProcess().Handle,
                desiredAccess: Privileges.TokenAdjustPrivileges | Privileges.TokenQuery,
                tokenHandle: out var tokenHandle))
            return tokenHandle;
        
        _logger.LogError("Failed to get process token: {tokenHandle}",
            tokenHandle);
        throw new Win32Exception();
    }
}