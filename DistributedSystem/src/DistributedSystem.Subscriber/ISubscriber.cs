namespace DistributedSystem.Subscriber;

public interface ISubscriber
{
    Task ConnectAsync(string ip, int port);
    Task StartReceiveAsync();
    Task ChangeTopicAsync(string topic);
}
