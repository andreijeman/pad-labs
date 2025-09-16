namespace DistributedSystem.Broker.Messages;

public static class MessageCommand
{
    public const string Subscribe = "subscribe";
    public const string Unsubscribe = "unsubscribe";
    
    public const string RegisterPublisher = "register-publisher";
    public const string Publish = "publish";
}
