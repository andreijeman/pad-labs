using DistributedSystem.Broker.Messages;
namespace DistributedSystem.Publisher;

public interface IPublisher
{
    Task ConnectAsync(Configuration configuration);
    Task SendAsync(Message message);
    
}
