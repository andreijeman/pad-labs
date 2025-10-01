using DistributedSystem.Broker;
using DistributedSystem.Broker.Messages;
using DistributedSystem.Common.PanelCommands;
using DistributedSystem.Network;
using DistributedSystem.Terminal;

var panel = new CommandPanel();
var broker = new Broker(panel, new Postman<Message>(new JsonCodec<Message>()));

panel.AddCommand(new StartBrokerCommand(panel, broker));

_ = panel.StartAsync();

// Uncomment for Automated Setup
// panel.ExecuteTextCommand($"start -i {NetworkHelper.GetLocalIPv4()} -p 7777 -m 8 -d 50");

await Task.Delay(-1);

