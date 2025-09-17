using System.Net;

namespace DistributedSystem.Publisher;

public class Configuration
{
    public IPAddress IpAddress { get; set; } = null!;
    public int Port { get; set; }
}
