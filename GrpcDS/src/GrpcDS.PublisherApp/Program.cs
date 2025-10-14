using GrpcDS.Broker.Messages;
using GrpcDS.Common.PanelCommands;
using GrpcDS.Network;
using GrpcDS.Publisher;
using GrpcDS.Terminal;

var panel = new CommandPanel();

var publisher = new Publisher(new Postman<Message>(new JsonCodec<Message>()), panel);

panel.AddCommand(new ConnectCommand(panel, publisher));
panel.AddCommand(new AuthenticateCommand(panel, publisher));
panel.AddCommand(new RegisterPublisherCommand(panel, publisher));
panel.AddCommand(new PublishCommand(panel, publisher));

_ = panel.StartAsync();

// Automated connection and setup
// panel.ExecuteTextCommand($"conn -i {NetworkHelper.GetLocalIPv4()} -p 7777");
// panel.ExecuteTextCommand($"auth -u publisher");
// panel.ExecuteTextCommand($"reg -p publisher");
//
// panel.LogInfo("Use command [pub -m <message>] to publish messages.");
//
await Task.Delay(-1);

