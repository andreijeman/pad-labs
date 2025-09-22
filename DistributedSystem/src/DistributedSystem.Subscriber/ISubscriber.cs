namespace DistributedSystem.Subscriber;

public interface ISubscriber
{
    Task ConnectAsync(ConnectionArgs configuration);
    Task StartReceiveAsync();
    Task ChangeTopicAsync(string topic);
    bool IsConnected();
}
