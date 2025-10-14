using GrpcDS.Client;
using GrpcDS.Broker.Messages;
using GrpcDS.Broker.Client;
using GrpcDS.Logger;
using GrpcDS.Network;

namespace GrpcDS.Publisher;

public class Publisher : Client.Client, IPublisher
{
    public Publisher(IPostman<Message> postman, ILogger logger) : base(postman, logger)
    {
    }
    
    public Task RegisterPubliher(string name)
    {
        return SendMessageAsync(new Message { Code = MessageCode.RegisterPublisher, Body = name });
    }
}
