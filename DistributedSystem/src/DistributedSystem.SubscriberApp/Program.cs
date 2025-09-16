using DistributedSystem.Logger;
using DistributedSystem.Subscriber;
using System.Reflection.Metadata.Ecma335;

Console.WriteLine("__ SUBSCRIBER __");

Console.Write("Subscribe to topic: ");
string topic;

while(true)
{
    topic = Console.ReadLine();
    if (!string.IsNullOrEmpty(topic)) break;
    continue;
}

ILogger logger = new ConsoleLogger();
ISubscriber subscriber = new Subscriber(logger, topic);

// TODO - Connect to broker
// TODO - Receive from broker