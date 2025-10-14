using GrpcDS.Broker.Messages;

namespace GrpcDS.Broker.Client;

public interface IClient
{
    Task ConnectAsync(ConnectionArgs args);
    Task AuthenticateAsync(AuthenticationArgs args);
    Task SendMessageAsync(Message message);
    event EventHandler<Message> MessageReceived;
}