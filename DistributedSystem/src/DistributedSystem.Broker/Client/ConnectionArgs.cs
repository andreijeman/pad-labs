using System.Net;

namespace DistributedSystem.Broker.Client;

public class ConnectionArgs
{
    public required IPAddress IpAddress{ get; set; }
    public int Port { get; set; }
}