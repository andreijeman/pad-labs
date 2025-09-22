using System.Net;

namespace DistributedSystem.Publisher;

public class ConnectionArgs
{
    public IPAddress IpAddress { get; set; } = null!;
    public int Port { get; set; }
}
