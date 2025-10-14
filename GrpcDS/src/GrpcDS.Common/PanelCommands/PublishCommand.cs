using GrpcDS.Broker.Client;
using GrpcDS.Broker.Messages;
using GrpcDS.Terminal;
using GrpcDS.Terminal.DefaultCommands;

namespace GrpcDS.Common.PanelCommands;

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