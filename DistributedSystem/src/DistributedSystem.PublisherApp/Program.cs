using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using DistributedSystem.Publisher;
using DistributedSystem.Terminal;



var panel = new CommandPanel();

var pub = new Publisher(new Postman<Message>(new JsonCodec<Message>()), panel);


panel.Start();