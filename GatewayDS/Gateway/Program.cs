using Gateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapHealthChecksEndpoints();

app.UseOutputCache();
app.MapReverseProxy().CacheOutput("users-cache");

app.Run();

