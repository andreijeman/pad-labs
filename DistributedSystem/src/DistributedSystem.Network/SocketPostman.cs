using System.Net.Sockets;

namespace DistributedSystem.Network;

public class SocketPostman<TPacket> : IPostman<TPacket>
{
    private ICodec<TPacket> _codec;
    private byte[] _buffer;

    public SocketPostman(ICodec<TPacket> codec, int bufferSize = 1024)
    {
        _codec = codec;
        _buffer = new byte[bufferSize];
    }

    public async Task<TPacket> ReceivePacketAsync(Socket socket)
    {
        int count = await socket.ReceiveAsync(_buffer);
        return _codec.Unpack(_buffer, 0, count);
    }

    public async Task SendPacketAsync(Socket socket, TPacket data)
    {
        await socket.SendAsync(_codec.Pack(data));
    }
}