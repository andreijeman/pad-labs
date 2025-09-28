using DistributedSystem.Broker.Client;
using DistributedSystem.Terminal;
using DistributedSystem.Terminal.DefaultCommands;

namespace DistributedSystem.Common.PanelCommands;

public class AuthCommand : PanelCommandBase
{
    private readonly IClient _client;
    
    public AuthCommand(ICommandPanel panel, IClient client) : base(panel)
    {
        _client = client;
        
        Name = "auth";
        Description = new CommandDescriptionBuilder
        {
            Name = this.Name,
            Description = "Authenticate to the broker",
            Signature = "auth [-u username]"
        }.Build();
    }

    public override async Task Execute(Dictionary<string, string> args)
    {
        if (args.TryGetValue("-u", out var username))
        {
            await _client.AuthenticateAsync(new AuthenticationArgs { Username = username });
        }
        else Panel.LogWarning("Invalid command args");
    }
}