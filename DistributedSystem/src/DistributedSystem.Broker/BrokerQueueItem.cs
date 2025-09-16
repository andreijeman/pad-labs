using System.Net.Sockets;
using DistributedSystem.Broker.Messages;

namespace DistributedSystem.Broker;

public class BrokerQueueItem
{
    public required Socket Sender { get; set; }
    public required Message Message { get; set; }
}