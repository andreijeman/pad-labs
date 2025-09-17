namespace DistributedSystem.Subscriber;

public interface ISubscriber
{
    Task ConnectAsync(Configuration configuration);
    Task StartReceiveAsync();
    Task ChangeTopicAsync(string topic);
}
