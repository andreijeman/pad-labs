using System.Net.Sockets;
using DistributedSystem.Broker.Messages;

namespace DistributedSystem.Broker;

public class BrokerQueueItem
{
    public Socket Sender { get; }
    public Message Message { get; }

    public BrokerQueueItem(Socket sender, Message message)
    {
        Sender = sender;
        Message = message;
    }
}