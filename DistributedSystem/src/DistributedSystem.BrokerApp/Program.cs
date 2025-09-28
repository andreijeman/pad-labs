using DistributedSystem.Broker;
using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;

var logger = new ConsoleLogger();

var conf = new BrokerArgs
{
    IpAddress = NetworkHelper.GetLocalIPv4(),
    Port = 7777,
    MaxConnections = 7,
    QueueHandlerDelay = 500,
};

var broker = new Broker(
    conf, 
    logger, 
    new Postman<Message>(new JsonCodec<Message>())
);

broker.Start();
logger.LogInfo($"Broker started on ip {conf.IpAddress.ToString()} and port {conf.Port}");

await Task.Delay(-1);