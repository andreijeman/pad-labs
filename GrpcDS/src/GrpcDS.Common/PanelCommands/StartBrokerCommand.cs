using System.Net;
using GrpcDS.Broker;
using GrpcDS.Terminal;
using GrpcDS.Terminal.DefaultCommands;

namespace GrpcDS.Common.PanelCommands;

public class StartBrokerCommand : PanelCommandBase
{
    private readonly IBroker _broker;
    
    public StartBrokerCommand(ICommandPanel panel, IBroker broker) : base(panel)
    {
        _broker = broker;
        
        Name = "start";
        Description = new CommandDescriptionBuilder
        {
            Name = this.Name,
            Description = "Start broker",
            Signature = "start [-i <ip-address>] [-p <port>] [-m <max-connections>] [-d <queue-handler-delay>]"
        }.Build();
    }

    public override Task Execute(Dictionary<string, string> args)
    {
        var brokerArgs = new BrokerArgs();
        
        if (args.TryGetValue("-i", out var ipStr) && IPAddress.TryParse(ipStr, out var ip)) 
            brokerArgs.IpAddress = ip;

        if (args.TryGetValue("-p", out var portStr) && int.TryParse(portStr, out var port))
            brokerArgs.Port = port;
        
        if (args.TryGetValue("-m", out var maxStr) && int.TryParse(maxStr, out var max))
            brokerArgs.MaxConnections = max;
        
        if (args.TryGetValue("-d", out var delayStr) && int.TryParse(delayStr, out var delay))
            brokerArgs.QueueHandlerDelay = delay;
        
        _broker.Start(brokerArgs);
        
        Panel.LogInfo($"Broker started on ip {brokerArgs.IpAddress} and port {brokerArgs.Port}");
        
        return Task.CompletedTask;
    }
}