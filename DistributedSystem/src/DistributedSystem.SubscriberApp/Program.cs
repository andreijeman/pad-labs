using DistributedSystem.Broker.Messages;
using DistributedSystem.Common.Validators;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using DistributedSystem.Subscriber;
using System.Net;

Console.WriteLine("__ SUBSCRIBER __");

IPAddress ipAddress = InputValidator.ValidateInput( "IP Broker: ", input => (IPAddress.TryParse(input, out var ip), ip), "Invalid IP Address")!;
int port = InputValidator.ValidateInput( "PORT Broker: ", input => (int.TryParse(input, out var port),  port), "Invalid port")!;
string topic = InputValidator.ValidateInput("Subscribe to topic: ", input => (!string.IsNullOrWhiteSpace(input), input), "Empty field!");

ISubscriber subscriber = new Subscriber(new Postman<Message>(new JsonCodec<Message>()), new ConsoleLogger(), topic);

await subscriber.ConnectAsync(new Configuration { IpAddress = ipAddress, Port = port });

_ = Task.Run(subscriber.StartReceiveAsync);

Console.ReadLine();