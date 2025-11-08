using Api.Domain;
using Api.Options;
using Api.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Extensions;

public static class InfrastructureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<MongoDbSettings>(provider => provider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<MongoDbSettings>();
            return new MongoClient(settings.DefaultConnection);
        });

        services.AddScoped<IRepository<User>, MongoRepository<User>>();

        services.AddMongoHealthChecks(configuration);
    }

    private static void AddMongoHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddMongoDb(sp => sp.GetRequiredService<IMongoClient>(), name: "mongo");
    }
}