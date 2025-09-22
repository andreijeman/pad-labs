using DistributedSystem.Broker.Messages;
using DistributedSystem.Common.Validators;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using DistributedSystem.Subscriber;
using System.Net;

Console.WriteLine("__ SUBSCRIBER __");

IPAddress ipAddress = InputValidator.ValidateInput("Broker IP: ", input => (IPAddress.TryParse(input, out var ip), ip), "Invalid IP Address")!;
int port = InputValidator.ValidateInput("Broker PORT: ", input => (int.TryParse(input, out var port), port), "Invalid port")!;

string name;
ISubscriber subscriber;

while (true)
{
    name = InputValidator.ValidateInput("Subscriber Name: ", input => (!string.IsNullOrWhiteSpace(input), input), "Empty field!");
    subscriber = new Subscriber(new Postman<Message>(new JsonCodec<Message>()), new ConsoleLogger(), name);
    
    await subscriber.ConnectAsync(new Configuration { IpAddress = ipAddress, Port = port });
    
    if (subscriber.IsConnected())
    {
        _ = Task.Run(subscriber.StartReceiveAsync);
        break;
    }
}

while (true)
{
    string newTopic = InputValidator.ValidateInput(String.Empty, input => (!string.IsNullOrWhiteSpace(input), input), "Empty field!");

    await subscriber.ChangeTopicAsync(newTopic);
    Console.WriteLine($"Topic changed to: {newTopic}");
}