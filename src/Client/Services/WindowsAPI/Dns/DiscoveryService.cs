using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Ananke.Extensions;
using Ananke.Services.WindowsAPI.Handles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ananke.Services.WindowsAPI.Dns;

// https://github.com/openmediatransport/libomtnet/blob/d2a62512c595cea4e2ed44e59a46ddcd45765caa/src/win32/DnsApi.cs
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class DiscoveryService : IDiscoveryService
{
    [DllImport("dnsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int DnsServiceRegister(
        ref PDNS_SERVICE_REGISTER_REQUEST pRequest,
        DnsCancelHandle pCancel);

    [DllImport("dnsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int DnsServiceDeRegister(
        ref PDNS_SERVICE_REGISTER_REQUEST pRequest,
        DnsCancelHandle pCancel);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void DnsServiceRegisterComplete(
        uint status,
        IntPtr pQueryContext,
        IntPtr pInstanceName);

    private readonly ILogger<DiscoveryService> _logger;
    private readonly IOptionsMonitor<DiscoveryStartupOption> _options;

    public DiscoveryService(
        ILogger<DiscoveryService> logger,
        IOptionsMonitor<DiscoveryStartupOption> options)
    {
        _logger = logger;
        _options = options;
    }

    // To change mdns service properties, modify appsettings.json
    public void SetupMulticastDnsService()
    {
        var serviceInstance = CreateServiceInstance();

        var request = new PDNS_SERVICE_REGISTER_REQUEST
        {
            Version = 1,
            InterfaceIndex = 0,
            pServiceInstance = Marshal.AllocHGlobal(
                Marshal.SizeOf(serviceInstance)),
            pRegisterCompletionCallback = Marshal.GetFunctionPointerForDelegate(
                new DnsServiceRegisterComplete(OnCompleted)),
            unicastEnabled = false
        };

        try
        {
            Marshal.StructureToPtr(
                structure: serviceInstance,
                ptr: request.pServiceInstance,
                fDeleteOld: false);

            RegisterDnsService(
                request);
        }
        finally
        {
            Marshal.FreeHGlobal(
                request.pServiceInstance);
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
    
    private void RegisterDnsService(
        PDNS_SERVICE_REGISTER_REQUEST request)
    {
        var windowsResponseCode = DnsServiceRegister(
            pRequest: ref request,
            pCancel: new DnsCancelHandle());

        if (windowsResponseCode != (int)WindowsErrorCodes.DNS_REQUEST_PENDING)
        {
            _logger.LogError(
                "Failed to register DNS service. Windows response code: {ErrorCode} " +
                "Last error message: {ErrorMessage}.", 
                windowsResponseCode, Marshal.GetLastWin32Error());
        }
    }

    private static void OnCompleted(
        uint status,
        IntPtr pQueryContext,
        IntPtr pInstanceName)
    {
    }
}