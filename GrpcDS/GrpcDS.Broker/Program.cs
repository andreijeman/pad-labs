using Grpc.Broker.Services;
using Grpc.Broker.Interfaces;
using GrpcDS.Common;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(EndpointConsts.BrokerAddress);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddSingleton<IMessageStorageService, MessageStorageService>();
builder.Services.AddSingleton<IConnectionStorageService, ConnectionStorageService>();
builder.Services.AddHostedService<SenderWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => "Broker is up");
app.MapGrpcService<PublisherService>();
app.MapGrpcService<SubscriberService>();
app.MapControllers();

app.Run();
