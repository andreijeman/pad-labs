using GrpcDS.Broker.Client;
using GrpcDS.Terminal;
using GrpcDS.Terminal.DefaultCommands;

namespace GrpcDS.Common.PanelCommands;

public class RegisterPublisherCommand : PanelCommandBase
{
    private readonly IPublisher _publisher;
    
    public RegisterPublisherCommand(ICommandPanel panel, IPublisher publisher) : base(panel)
    {
        _publisher = publisher;
        
        Name = "reg";
        Description = new CommandDescriptionBuilder
        {
            Name = this.Name,
            Description = "Register publisher to the broker",
            Signature = "reg [-p <publisher-name>]"
        }.Build();
    }

    public override async Task Execute(Dictionary<string, string> args)
    {
        if (args.TryGetValue("-p", out var name))
        {
            await _publisher.RegisterPubliher(name);
        }
        else Panel.LogWarning("Invalid command args");
    }
}