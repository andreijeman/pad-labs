using GrpcDS.Broker.Messages;
using GrpcDS.Common.PanelCommands;
using GrpcDS.Network;
using GrpcDS.Subscriber;
using GrpcDS.Terminal;

var panel = new CommandPanel();

var subscriber = new Subscriber(new Postman<Message>(new JsonCodec<Message>()), panel);

panel.AddCommand(new ConnectCommand(panel, subscriber));
panel.AddCommand(new AuthenticateCommand(panel, subscriber));
panel.AddCommand(new SubscribeCommand(panel, subscriber));

subscriber.MessageReceived += (sender, message) =>
{
    panel.ShowMessageAction(() =>
    {
        if(message.Code == MessageCode.Publish) 
            Console.WriteLine("Publication: " + message.Body);
    });
};

_ = panel.StartAsync();

// Uncomment for Automated Connection & Setup
// panel.ExecuteTextCommand($"conn -i {NetworkHelper.GetLocalIPv4()} -p 7777");
// panel.ExecuteTextCommand("auth -u subscriber");
// panel.ExecuteTextCommand("sub -p publisher");
//
// panel.LogInfo("Use command [sub -p <publisher-name>] to subscribe to publisher.");

await Task.Delay(-1);
