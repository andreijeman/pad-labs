namespace DistributedSystem.Broker.Client;

public interface ISubscriber
{
    Task<bool> Subscribe(string publisher);
    Task<bool> Unsubscribe(string publisher);
}