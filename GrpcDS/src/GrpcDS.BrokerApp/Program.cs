using GrpcDS.Broker;
using GrpcDS.Broker.Messages;
using GrpcDS.Common.PanelCommands;
using GrpcDS.Network;
using GrpcDS.Terminal;

var panel = new CommandPanel();
var broker = new Broker(panel, new Postman<Message>(new JsonCodec<Message>()));

panel.AddCommand(new StartBrokerCommand(panel, broker));

_ = panel.StartAsync();

// Uncomment for Automated Setup
// panel.ExecuteTextCommand($"start -i {NetworkHelper.GetLocalIPv4()} -p 7777 -m 8 -d 50");

await Task.Delay(-1);

