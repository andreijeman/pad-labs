using DistributedSystem.Broker.Messages;

namespace DistributedSystem.Broker.Client;

public interface IClient
{
    Task<bool> ConnectAsync(ConnectionArgs args);
    Task<bool> AuthenticateAsync(AuthenticationArgs args);
    Task SendMessageAsync(Message message);
}