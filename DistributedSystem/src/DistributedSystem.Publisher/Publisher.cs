using DistributedSystem.Broker.Messages;
using DistributedSystem.Broker.Client;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Publisher;

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
