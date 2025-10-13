using Ananke.WindowsAPI;

namespace Ananke.Services;

internal interface IProcessTokenManager
{
    void SetProcessPrivilege(
        string privilegeName = SEPrivileges.SE_SHUTDOWN_NAME,
        int privilegeCount = SEPrivileges.SE_PRIVILEGE_COUNT,
        uint privilegeAttributes = SEPrivileges.SE_PRIVILEGE_ENABLED,
        bool disableAllPrivileges = false);
    
    bool DisableHandle();
}