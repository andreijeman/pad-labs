using GrpcDS.Subscriber.Services;

var service = new SubscriberService();

await service.StartAsync();

await service.Subscribe();
Console.ReadLine();

await service.StopAsync();