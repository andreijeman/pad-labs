namespace DistributedSystem.Broker;

public class BrokerArgs
{
    public int IpAddress { get; init; }
    public int Port { get; init; }
    public int MaxConnections { get; init; }
    public int QueueHandlerDelay { get; init; }
}