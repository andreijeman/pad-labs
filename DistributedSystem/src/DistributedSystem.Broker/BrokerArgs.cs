namespace DistributedSystem.Broker;

public class BrokerArgs
{
    public int IpAddress { get; init; }
    
    public int PublisherPort { get; init; }
    public int PublisherMaxConnections { get; init; }
    
    public int SubscriberPort { get; init; }
    public int SubscriberMaxConnections { get; init; }
}