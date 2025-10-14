using GrpcDS.Broker.Messages;
using GrpcDS.Broker.Client;
using GrpcDS.Logger;
using GrpcDS.Network;

namespace GrpcDS.Subscriber;

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
