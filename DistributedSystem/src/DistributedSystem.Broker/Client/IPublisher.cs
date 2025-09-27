namespace DistributedSystem.Broker.Client;

public interface IPublisher
{
    Task<bool> RegisterPubliher(string name);
}