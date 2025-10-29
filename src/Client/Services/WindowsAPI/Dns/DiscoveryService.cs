using Ananke.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ananke.Services.WindowsAPI.Dns;

// https://github.com/openmediatransport/libomtnet/blob/d2a62512c595cea4e2ed44e59a46ddcd45765caa/src/win32/DnsApi.cs
public class DiscoveryService : IDiscoveryService
{
    private readonly ILogger<DiscoveryService> _logger;
    private readonly IOptionsMonitor<DiscoveryStartupOption> _optionsMonitor;

    public DiscoveryService(
        ILogger<DiscoveryService> logger,
        IOptionsMonitor<DiscoveryStartupOption> optionsMonitor)
    {
        _logger = logger;
        _optionsMonitor = optionsMonitor;
    }

    public void StartServiceDiscovery()
    {
    }

    public void StopServiceDiscovery()
    {
    }
}