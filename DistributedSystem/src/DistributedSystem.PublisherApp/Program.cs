using System.Globalization;
using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using DistributedSystem.Publisher;

var connArgs = new ConnectionArgs
{
    IpAddress = NetworkHelper.GetLocalIPv4(),
    Port = 7777
};

var publisher = new Publisher(
    DateTime.UtcNow.Millisecond.ToString(CultureInfo.InvariantCulture), 
    new Postman<Message>(new JsonCodec<Message>()),
    new ConsoleLogger());

await publisher.ConnectAsync(connArgs);

Console.Write("Name of publisher: ");
var pubName = Console.ReadLine();

await publisher.RegisterPubliher(pubName!);

while(publisher.Connected)
{
    string message = Console.ReadLine()!;
    await publisher.SendMessageAsync(new Message { Command = MessageCommand.Publish, Body = message});
}

Console.ReadLine();
