using System.Net;
using DistributedSystem.Network;

namespace DistributedSystem.Broker.Client;

public class ConnectionArgs
{
    public IPAddress IpAddress { get; set; } = NetworkHelper.GetLocalIPv4();
    public int Port { get; set; } = 7777;
}