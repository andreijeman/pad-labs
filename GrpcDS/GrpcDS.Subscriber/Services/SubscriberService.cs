using GrpcDS.Common;
using Grpc.Net.Client;
using Grpc.Core;
using GrpcAgent;

namespace GrpcDS.Subscriber.Services;

public class SubscriberService
{
    private Server? _grpcServer;
    public async Task StartAsync()
    {
        const int port = 0;

        _grpcServer = new Server
        {
            Services = { Notifier.BindService(new NotifierService()) },
            Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
        };

        _grpcServer.Start();

        var actualPort = _grpcServer.Ports.First().BoundPort;

        await Task.CompletedTask;
    }

    public async Task Subscribe()
    {
        if (_grpcServer is null)
        {
            return;
        }

        var actualPort = _grpcServer.Ports.First().BoundPort;
        var subscriberAddress = $"http://localhost:{actualPort}";

        //Console.WriteLine($"Subscriber address: {subscriberAddress}");
        Console.Write("Topic: ");
        var topic = Console.ReadLine()?.Trim().ToLower();

        if (string.IsNullOrEmpty(topic))
        {
            Console.WriteLine("Error: Topic cannot be empty!");
            return;
        }

        var channel = GrpcChannel.ForAddress(EndpointConsts.BrokerAddress);
        var client = new GrpcAgent.Subscriber.SubscriberClient(channel);

        var request = new SubscribeRequest
        {
            Topic = topic,
            Address = subscriberAddress
        };

        try
        {
            var reply = await client.SubscribeAsync(request);

        }
        catch (RpcException ex)
        {
            Console.WriteLine($"gRPC Error: {ex.Status.Detail}");
            Console.WriteLine($"Status Code: {ex.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error subscribing: {ex.Message}");
        }
    }

    public async Task StopAsync()
    {
        if (_grpcServer != null)
        {
            await _grpcServer.ShutdownAsync();
        }
    }
}