using System.Net;
using DistributedSystem.Network;

namespace DistributedSystem.Broker;

public class BrokerArgs
{
    public IPAddress IpAddress { get; set; } = NetworkHelper.GetLocalIPv4();
    public int Port { get; set; } = 7777;
    public int MaxConnections { get; set; } = 64;
    public int QueueHandlerDelay { get; set; } = 25;
}