using DistributedSystem.Broker.Messages;

namespace DistributedSystem.Broker.Client;

public interface IClient
{
    Task ConnectAsync(ConnectionArgs args);
    Task AuthenticateAsync(AuthenticationArgs args);
    Task SendMessageAsync(Message message);
    event EventHandler<Message> MessageReceived;
}