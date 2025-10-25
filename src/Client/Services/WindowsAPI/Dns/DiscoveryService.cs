using System.Runtime.InteropServices;
using Ananke.Services.WindowsAPI.Handles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ananke.Services.WindowsAPI.Dns;

// https://github.com/openmediatransport/libomtnet/blob/d2a62512c595cea4e2ed44e59a46ddcd45765caa/src/win32/DnsApi.cs
public class DiscoveryService : IHostedService
{
    [DllImport("dnsapi.dll", SetLastError = true)]
    private static extern int DnsServiceRegister(
        ref PDNS_SERVICE_REGISTER_REQUEST pRequest,
        DnsCancelHandle pCancel);

    private readonly ILogger<DiscoveryService> _logger;
    
    public DiscoveryService(
        ILogger<DiscoveryService> logger)
    {
        _logger = logger;
    }
    
    public Task StartAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}