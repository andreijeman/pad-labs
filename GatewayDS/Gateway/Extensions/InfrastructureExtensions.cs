using Gateway.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Gateway.Extensions;

public static class InfrastructureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        services.AddOutputCache(o =>
        {
            o.AddPolicy("users-cache", p =>
                p.Expire(TimeSpan.FromSeconds(30))
                    .SetVaryByQuery("*")
                    .Tag("users"));

            o.AddPolicy("default-cache", p =>
                p.Expire(TimeSpan.FromMinutes(5))
                    .SetVaryByQuery("*"));
        });

        services.AddHealthChecks()
            .AddCheck("gateway-self", () => HealthCheckResult.Healthy(), tags: new[] { "gateway" });

        var dependencies = configuration.GetSection("HealthChecks:Dependencies").Get<List<DependencyCfg>>();
        foreach (var d in dependencies!)
        {
            services.AddHealthChecks().AddUrlGroup(
                uri: new Uri(d.Url),
                name: d.Name,
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(5));
        }
    }
}