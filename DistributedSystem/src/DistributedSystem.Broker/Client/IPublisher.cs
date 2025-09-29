namespace DistributedSystem.Broker.Client;

public interface IPublisher
{
    Task RegisterPubliher(string name);
}