using DistributedSystem.Broker.Messages;
using System.Net.Sockets;
using DistributedSystem.Broker.Client;
using DistributedSystem.Common;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Subscriber;

public class Subscriber : Client.Client, ISubscriber
{
    public Subscriber(IPostman<Message> postman, ILogger logger) :  base(postman, logger)
    {
    }

    public Task<bool> Subscribe(string publisher)
    {
        throw new NotImplementedException();
    }
}
