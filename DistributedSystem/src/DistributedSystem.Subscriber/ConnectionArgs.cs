using System.Net;

namespace DistributedSystem.Subscriber;

public class ConnectionArgs
{
    public IPAddress IpAddress { get; set; } = null!;
    public int Port { get; set; }
}
