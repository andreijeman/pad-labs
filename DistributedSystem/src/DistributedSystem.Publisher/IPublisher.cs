using DistributedSystem.Broker.Messages;
namespace DistributedSystem.Publisher;

public interface IPublisher
{
    Task ConnectAsync(ConnectionArgs connectionArgs);
    Task SendMessageAsync(Message message);
    Task RegisterPubliher(string name);
    
}
