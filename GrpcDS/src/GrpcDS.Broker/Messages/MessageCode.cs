namespace GrpcDS.Broker.Messages;

public static class MessageCode
{
    public const string Ok = "ok";
    public const string Fail = "fail";
    
    public const string Subscribe = "sub";
    public const string Unsubscribe = "unsub";
    
    public const string RegisterPublisher = "rpub";
    public const string Publish = "pub";
    
    public const string Authenticate = "auth";
    
}
