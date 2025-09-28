namespace DistributedSystem.Broker.Client;

public interface ISubscriber
{
    Task<bool> Subscribe(string publisher);
}