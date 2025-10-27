using System.ComponentModel.DataAnnotations;

namespace Ananke.Services.WindowsAPI.Dns;

public sealed class DiscoveryStartupOption
{
    public const string SectionName = "DiscoveryOptions";
    public required string DnsInstanceName { get; set; }
    public required ushort Port { get; set; }
    public ushort Weight { get; set; } = 0;
    public ushort Priority { get; set; } = 0;
}