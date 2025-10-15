namespace Ananke.Services.WindowsAPI;

/// <summary>
/// Privilege Constants for the Windows Process Token
/// Add tokens from here: https://learn.microsoft.com/en-us/windows/win32/secauthz/privilege-constants
/// </summary>
internal static class SEPrivileges
{
    public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
    
    public const int SE_PRIVILEGE_ENABLED = 2;
    
    public const int SE_PRIVILEGE_COUNT = 1;
}