using DistributedSystem.Broker;
using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;

var brokerArgs = new BrokerArgs
{
    IpAddress = NetworkHelper.GetLocalIPv4(),
    Port = 7777,
    MaxConnections = 7,
    QueueHandlerDelay = 500,
};

var broker = new Broker(brokerArgs, new ConsoleLogger(), new Postman<Message>(new JsonCodec<Message>()));

Console.WriteLine("Broker started!");
broker.Start();

await Task.Delay(-1);
