namespace GrpcDS.Broker.Messages;

public class Message
{
    public string Code { get; set; } = MessageCode.Fail;
    public string Body { get; set; } = "";
}