using DistributedSystem.Broker.Messages;
using DistributedSystem.Broker.Client;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Subscriber;

public class Subscriber : Client.Client, ISubscriber
{
    public Subscriber(IPostman<Message> postman, ILogger logger) :  base(postman, logger)
    {
    }

    public Task Subscribe(string publisher)
    {
        return SendMessageAsync(new Message { Code = MessageCode.Subscribe, Body = publisher });
    }

    public Task Unsubscribe(string publisher)
    {
        return SendMessageAsync(new Message { Code = MessageCode.Unsubscribe, Body = publisher });
    }
}
