using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using DistributedSystem.Subscriber;
using System.Net;

Console.WriteLine("__ SUBSCRIBER __");

IPAddress ipAddress = InputValidator.ValidateIp("IP Broker: ");
int port = InputValidator.ValidatePort("PORT Broker: ");
string topic = InputValidator.ValidateTopic("Subscribe to topic: ");

ISubscriber subscriber = new Subscriber(new Postman<Message>(new JsonCodec<Message>()), new ConsoleLogger(), topic);

Configuration configuration = new Configuration()
{
    IpAddress = ipAddress,
    Port = port
};

await subscriber.ConnectAsync(configuration);

_ = Task.Run(subscriber.StartReceiveAsync);

Console.ReadLine();