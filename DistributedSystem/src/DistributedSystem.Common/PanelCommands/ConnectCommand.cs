using System.Net;
using DistributedSystem.Broker.Client;
using DistributedSystem.Terminal;
using DistributedSystem.Terminal.DefaultCommands;

namespace DistributedSystem.Common.PanelCommands;

public class ConnectCommand : PanelCommandBase
{
    private readonly IClient _client;
    
    public ConnectCommand(ICommandPanel panel, IClient client) : base(panel)
    {
        _client = client;
        
        Name = "conn";
        Description = new CommandDescriptionBuilder
        {
            Name = this.Name,
            Description = "Connect to the broker",
            Signature = "conn [-i x.x.x.x] [-p n]"
        }.Build();
    }

    public override async Task Execute(Dictionary<string, string> args)
    {
        if (args.TryGetValue("-i", out var ipString) &&
            IPAddress.TryParse(ipString, out var ip) &&
            args.TryGetValue("-p", out var portString) &&
            int.TryParse(portString, out var port)
           )
        {
            await _client.ConnectAsync(new ConnectionArgs
            {
                IpAddress = ip,
                Port = port
            });
        }
        else Panel.LogWarning("Invalid command args");
    }
}