using DistributedSystem.Broker.Client;
using DistributedSystem.Terminal;
using DistributedSystem.Terminal.DefaultCommands;

namespace DistributedSystem.Common.PanelCommands;

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
            Signature = "reg [-n name]"
        }.Build();
    }

    public override async Task Execute(Dictionary<string, string> args)
    {
        if (args.TryGetValue("-n", out var name))
        {
            await _publisher.RegisterPubliher(name);
        }
        else Panel.LogWarning("Invalid command args");
    }
}