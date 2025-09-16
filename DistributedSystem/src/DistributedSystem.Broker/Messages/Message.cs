namespace DistributedSystem.Broker.Messages;

public class Message
{
    public required string Command { get; set; } 
    public required string Body { get; set; }
}