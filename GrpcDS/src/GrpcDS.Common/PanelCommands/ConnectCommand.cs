using System.Net;
using GrpcDS.Broker.Client;
using GrpcDS.Terminal;
using GrpcDS.Terminal.DefaultCommands;

namespace GrpcDS.Common.PanelCommands;

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
            Signature = "conn [-i <ip-address>] [-p <port>]"
        }.Build();
    }

    public override async Task Execute(Dictionary<string, string> args)
    {
        var connArgs = new ConnectionArgs();
        
        if (args.TryGetValue("-i", out var ipStr) && IPAddress.TryParse(ipStr, out var ip))
            connArgs.IpAddress = ip;
        
        if (args.TryGetValue("-p", out var portStr) && int.TryParse(portStr, out var port))
            connArgs.Port = port;

        await _client.ConnectAsync(connArgs);
    }
}