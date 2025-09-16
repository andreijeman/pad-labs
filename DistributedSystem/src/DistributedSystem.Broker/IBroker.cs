namespace DistributedSystem.Broker;

public interface IBroker
{ 
    void Start(CancellationToken cancellationToken = default);
}

