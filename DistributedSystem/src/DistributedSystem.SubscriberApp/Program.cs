using DistributedSystem.Broker.Messages;
using DistributedSystem.Common.PanelCommands;
using DistributedSystem.Network;
using DistributedSystem.Subscriber;
using DistributedSystem.Terminal;

var panel = new CommandPanel();

var subscriber = new Subscriber(new Postman<Message>(new JsonCodec<Message>()), panel);

panel.AddCommand(new ConnectCommand(panel, subscriber));
panel.AddCommand(new AuthenticateCommand(panel, subscriber));
panel.AddCommand(new SubscribeCommand(panel, subscriber));

subscriber.MessageReceived += (sender, message) =>
{
    panel.ShowMessageAction(() =>
    {
        Console.WriteLine(message.Body);
    });
};

_ = Task.Run(() => panel.Start());


await Task.Delay(-1);