namespace GrpcDS.Network;

public interface ICodec<TPacket>
{
    public byte[] Pack(TPacket packet);
    public TPacket Unpack(byte[] data, int index, int count);
}