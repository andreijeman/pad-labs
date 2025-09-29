using DistributedSystem.Broker;
using DistributedSystem.Broker.Messages;
using DistributedSystem.Common.PanelCommands;
using DistributedSystem.Network;
using DistributedSystem.Terminal;

var panel = new CommandPanel();
var broker = new Broker(panel, new Postman<Message>(new JsonCodec<Message>()));

panel.AddCommand(new StartBrokerCommand(panel, broker));

panel.Start();
