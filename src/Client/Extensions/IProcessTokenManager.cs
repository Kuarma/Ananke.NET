using Ananke.Services.WindowsAPI;

namespace Ananke.Extensions;

public interface IProcessTokenManager
{
    public bool TrySetProcessPrivilege(
        string privilegeName = SEPrivileges.SE_SHUTDOWN_NAME,
        uint privilegeCount = SEPrivileges.SE_PRIVILEGE_COUNT,
        uint privilegeAttributes = SEPrivileges.SE_PRIVILEGE_ENABLED);

    public bool TryDisableHandle();
}