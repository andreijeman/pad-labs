using DistributedSystem.Broker.Messages;
using DistributedSystem.Common.PanelCommands;
using DistributedSystem.Network;
using DistributedSystem.Publisher;
using DistributedSystem.Terminal;

var panel = new CommandPanel();

var publisher = new Publisher(new Postman<Message>(new JsonCodec<Message>()), panel);

panel.AddCommand(new ConnectCommand(panel, publisher));
panel.AddCommand(new AuthenticateCommand(panel, publisher));
panel.AddCommand(new RegisterPublisherCommand(panel, publisher));
panel.AddCommand(new PublishCommand(panel, publisher));

panel.Start();