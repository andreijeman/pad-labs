using System.Text;
using System.Text.Json;

namespace Network;

public class JsonCodec<T> : ICodec<T>
    where T : class
{
    public byte[] Pack(T packet)
    {
        var jsonString = JsonSerializer.Serialize(packet);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    public T Unpack(byte[] data, int index, int count)
    {
        var jsonString = Encoding.UTF8.GetString(data, index, count);
        
        return JsonSerializer.Deserialize<T>(jsonString) ?? 
               throw new Exception("Unable to deserialize packet");
    }
}