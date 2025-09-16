using System.Net.Sockets;

namespace DistributedSystem.Network;
public interface IPostman<TPacket>
{
    Task<TPacket> ReceivePacketAsync(Socket socket);
    Task SendPacketAsync(Socket socket, TPacket data);
}
