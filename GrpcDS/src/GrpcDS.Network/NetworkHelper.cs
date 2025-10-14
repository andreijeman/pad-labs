using System.Net;
using System.Net.Sockets;

namespace GrpcDS.Network;

public static class NetworkHelper
{
    public static IPAddress GetLocalIPv4()
    {
        return Dns.GetHostAddresses(Environment.MachineName)
                  .First(a => a.AddressFamily == AddressFamily.InterNetwork);
    }
}
