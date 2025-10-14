using System.Net;
using GrpcDS.Network;

namespace GrpcDS.Broker.Client;

public class ConnectionArgs
{
    public IPAddress IpAddress { get; set; } = NetworkHelper.GetLocalIPv4();
    public int Port { get; set; } = 7777;
}