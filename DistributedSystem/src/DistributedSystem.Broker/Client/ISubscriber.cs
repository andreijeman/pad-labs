namespace DistributedSystem.Broker.Client;

public interface ISubscriber
{
    Task Subscribe(string publisher);
    Task Unsubscribe(string publisher);
}