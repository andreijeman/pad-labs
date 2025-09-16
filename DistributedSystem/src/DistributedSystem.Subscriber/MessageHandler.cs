using DistributedSystem.Broker.Messages;

namespace DistributedSystem.Subscriber;

public static class MessageHandler
{
    public static void Handler(Message message)
    {
        Console.WriteLine(message.Body);
    }
}
