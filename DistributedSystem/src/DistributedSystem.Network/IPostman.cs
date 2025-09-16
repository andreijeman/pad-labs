using System.Net.Sockets;

namespace DistributedSystem.Network;
public interface IPostman<TPacket>
{
    Task<TPacket> ReceivePacketAsync();
    Task SendPacketAsync(TPacket data);
}
