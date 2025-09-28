using DistributedSystem.Broker;
using DistributedSystem.Broker.Messages;
using Logger;
using Network;

var logger = new ConsoleLogger();

var broker = new Broker(
    new BrokerArgs
    {
        IpAddress = NetworkHelper.GetLocalIPv4(),
        Port = 7777,
        MaxConnections = 7,
        QueueHandlerDelay = 500,
    }, 
    logger, 
    new Postman<Message>(new JsonCodec<Message>())
);

broker.Start();
logger.LogInfo("Broker started!");

await Task.Delay(-1);