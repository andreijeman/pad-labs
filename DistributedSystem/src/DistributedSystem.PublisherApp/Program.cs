using DistributedSystem.Broker.Messages;
using DistributedSystem.Common.Validators;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using DistributedSystem.Publisher;
using System.Net;

Console.WriteLine("__ PUBLISHER __");

IPAddress ipAddress = InputValidator.ValidateInput("IP Broker: ", input => (IPAddress.TryParse(input, out var ip), ip), "Invalid IP Address")!;
int port = InputValidator.ValidateInput("PORT Broker: ", input => (int.TryParse(input, out var port), port), "Invalid port")!;
string topic = InputValidator.ValidateInput("Enter topic: ", input => (!string.IsNullOrWhiteSpace(input), input), "Empty field!");

IPublisher publisher = new Publisher(new Postman<Message>(new JsonCodec<Message>()), new ConsoleLogger(), topic);

await publisher.ConnectAsync(new ConnectionArgs { IpAddress = ipAddress, Port = port });

if (publisher.isConnected)
{
    while(true)
    {
        string message = InputValidator.ValidateInput("Enter message: ", input => (!string.IsNullOrWhiteSpace(input), input), "Empty field!");
        await publisher.SendMessageAsync(new Message { Command = MessageCommand.Publish, Body = message});
    }
}

Console.ReadLine();
