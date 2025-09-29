using DistributedSystem.Broker.Client;
using DistributedSystem.Broker.Messages;
using DistributedSystem.Terminal;
using DistributedSystem.Terminal.DefaultCommands;

namespace DistributedSystem.Common.PanelCommands;

public class PublishCommand : PanelCommandBase
{
    private readonly IClient _client;
    
    public PublishCommand(ICommandPanel panel, IClient client) : base(panel)
    {
        _client = client;
        
        Name = "pub";
        Description = new CommandDescriptionBuilder
        {
            Name = this.Name,
            Description = "Publish a message",
            Signature = "pub [-m <message>]"
        }.Build();
    }

    public override async Task Execute(Dictionary<string, string> args)
    {
        if (args.TryGetValue("-m", out var message))
        {
            await _client.SendMessageAsync(new Message { Code =  MessageCode.Publish, Body = message });
            Panel.ShowMessageAction(() => Console.WriteLine("You: " + message));
        }
        else Panel.LogWarning("Invalid command args");
    }
}