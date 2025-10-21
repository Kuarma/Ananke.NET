using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ananke.Services.WindowsAPI;

public class DiscoveryService : IHostedService
{
    [DllImport("dnsapi.dll", SetLastError = true)]
    private static extern int DnsServiceRegister(
        DnsServiceRegisterRequest pRequest,
        DnsServiceCancel pCancel);
    
    private readonly ILogger<DiscoveryService> _logger;
    
    public DiscoveryService(
        ILogger<DiscoveryService> logger)
    {
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}