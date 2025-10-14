namespace GrpcDS.Broker;

public interface IBroker
{ 
    void Start(BrokerArgs args, CancellationToken ct = default);
}

