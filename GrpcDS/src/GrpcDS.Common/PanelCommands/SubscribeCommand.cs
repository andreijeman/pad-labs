using GrpcDS.Broker.Client;
using GrpcDS.Terminal;
using GrpcDS.Terminal.DefaultCommands;

namespace GrpcDS.Common.PanelCommands;

public class SubscribeCommand : PanelCommandBase
{
    private readonly ISubscriber _subscriber;
    
    public SubscribeCommand(ICommandPanel panel, ISubscriber subscriber) : base(panel)
    {
        _subscriber = subscriber;
        
        Name = "sub";
        Description = new CommandDescriptionBuilder
        {
            Name = this.Name,
            Description = "Subscribe to publisher",
            Signature = "sub [-p <publisher-name>]"
        }.Build();
    }

    public override Task Execute(Dictionary<string, string> args)
    {
        if (args.TryGetValue("-p", out var publisher))
        {
            _ = _subscriber.Subscribe(publisher);
        }
        else Panel.LogWarning("Invalid command args");
        
        return Task.CompletedTask;
    }
}