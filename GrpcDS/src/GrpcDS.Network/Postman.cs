using System.Net.Sockets;

namespace GrpcDS.Network;

public class Postman<TPacket> : IPostman<TPacket>
{
    private ICodec<TPacket> _codec;
    private byte[] _buffer;

    public Postman(ICodec<TPacket> codec, int bufferSize = 1024)
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