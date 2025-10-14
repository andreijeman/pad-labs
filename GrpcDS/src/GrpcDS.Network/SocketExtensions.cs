using System.Net;
using System.Net.Sockets;

namespace GrpcDS.Network;

public static class SocketExtensions
{
    public static string GetIp(this Socket socket)
    {
        if (socket?.RemoteEndPoint != null)
        {
            return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
        }
        else throw new NullReferenceException();
    }

    public static void ShutdownAndClose(this Socket socket)
    {
        try
        {
            socket.Shutdown(SocketShutdown.Both);
        }
        finally
        {
            socket.Close();
        }
    }
}
