using System.Net;

namespace DistributedSystem.Subscriber;

public class Configuration
{
    public IPAddress IpAddress { get; set; } = null!;
    public int Port { get; set; }
}
