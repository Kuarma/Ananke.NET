using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ananke.Services;

public class DiscoveryService : IHostedService
{        
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