namespace GrpcDS.Broker.Client;

public interface IPublisher
{
    Task RegisterPubliher(string name);
}