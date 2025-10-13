namespace Ananke.WindowsAPI;

// https://learn.microsoft.com/en-us/windows/win32/secauthz/privilege-constants
internal static class SEPrivileges
{
    public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
    
    public const uint SE_PRIVILEGE_ENABLED = 0x2;
    
    public const int SE_PRIVILEGE_COUNT = 0x1;
}