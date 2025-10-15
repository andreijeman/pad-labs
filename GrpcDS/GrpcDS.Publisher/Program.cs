using Grpc.Net.Client;
using GrpcAgent;
using GrpcDS.Common;

var channel = GrpcChannel.ForAddress(EndpointConsts.BrokerAddress);
var client = new Publisher.PublisherClient(channel);

while (true)
{
    Console.Write("Topic: ");
    var topic = Console.ReadLine().ToLower();

    Console.Write("Content: ");
    var content = Console.ReadLine();

    var request = new PublishRequest { Topic = topic, Content = content };

    try
    {
        var reply = await client.PublishMessageAsync(request);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error publishing the message: {ex.Message}");
        continue;
    }
}