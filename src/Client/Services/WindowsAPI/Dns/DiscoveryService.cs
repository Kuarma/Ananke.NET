using System.Runtime.InteropServices;
using Ananke.Extensions;
using Ananke.Services.WindowsAPI.Handles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ananke.Services.WindowsAPI.Dns;

// https://github.com/openmediatransport/libomtnet/blob/d2a62512c595cea4e2ed44e59a46ddcd45765caa/src/win32/DnsApi.cs
public class DiscoveryService : IDiscoveryService
{
    [DllImport("dnsapi.dll", SetLastError = true)]
    private static extern int DnsServiceRegister(
        ref PDNS_SERVICE_REGISTER_REQUEST pRequest,
        DnsCancelHandle pCancel);

    [DllImport("dnsapi.dll", SetLastError = true)]
    private static extern int DnsServiceDeRegister(
        ref PDNS_SERVICE_REGISTER_REQUEST pRequest,
        DnsCancelHandle pCancel);

    private readonly ILogger<DiscoveryService> _logger;
    private readonly DnsServiceRegisterComplete _registerCallback;
    private readonly IOptionsMonitor<DiscoveryStartupOption> _options;

    public DiscoveryService(
        ILogger<DiscoveryService> logger,
        IOptionsMonitor<DiscoveryStartupOption> options)
    {
        _logger = logger;
        _registerCallback = OnCompleted;
        _options = options;
    }

    private void SetupMulticastDnsService()
    {
        var serviceInstance = CreateServiceInstance();
        
        var request = new PDNS_SERVICE_REGISTER_REQUEST
        {
            Version = 1,
            InterfaceIndex = 0,
            pServiceInstance = Marshal.AllocHGlobal(
                Marshal.SizeOf(serviceInstance)),
            pRegisterCompletionCallback = Marshal.GetFunctionPointerForDelegate(
                _registerCallback),
            unicastEnabled = false
        };
       
        Marshal.StructureToPtr(
            structure: serviceInstance,
            ptr: request.pServiceInstance,
            fDeleteOld: false);
        
        RegisterDnsService(
            request);

        Marshal.FreeHGlobal(
            request.pServiceInstance);
    }

    private void RegisterDnsService(
        PDNS_SERVICE_REGISTER_REQUEST request)
    {
        var windowsResponseCode = DnsServiceRegister(
            pRequest: ref request,
            pCancel: new DnsCancelHandle());

        if (windowsResponseCode != 9506)
        {
            _logger.LogError(
                "Failed to register DNS service. Windows response code: {ErrorCode} Error message: {ErrorMessage}. " +
                "Check the docs: https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-",
                windowsResponseCode, Marshal.GetLastWin32Error());
        }
    }

    private PDNS_SERVICE_INSTANCE CreateServiceInstance()
    {
        var discoveryOption = _options.CurrentValue;
        
        return new PDNS_SERVICE_INSTANCE
        {
            dwInterfaceIndex = 0,
            dwPropertyCount = 0,
            ip4Address = IntPtr.Zero,
            ip6Address = IntPtr.Zero,
            pszInstanceName = discoveryOption.DnsInstanceName,
            pszHostName = Environment.MachineName + ".local",
            wPort = discoveryOption.Port,
            wPriority = discoveryOption.Priority,
            wWeight = discoveryOption.Weight 
        };
    }
    
    private static void OnCompleted(
        uint status,
        IntPtr pQueryContext,
        IntPtr pInstanceName)
    {
    }
}