using System.Net;

namespace DistributedSystem.Broker;

public class BrokerArgs
{
    public required IPAddress IpAddress { get; init; }
    public int Port { get; init; }
    public int MaxConnections { get; init; }
    public int QueueHandlerDelay { get; init; }
}