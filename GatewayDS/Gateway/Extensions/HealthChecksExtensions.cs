using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Gateway.Extensions;

public static class HealthChecksExtensions
{
    public static IApplicationBuilder MapHealthChecksEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            },
            ResponseWriter = async (ctx, report) =>
            {
                ctx.Response.ContentType = "application/json";
                var payload = new
                {
                    status = report.Status.ToString(),
                    entries = report.Entries.ToDictionary(
                        e => e.Key,
                        e => new
                        {
                            status = e.Value.Status.ToString(),
                            //durationMs = e.Value.Duration.TotalMilliseconds
                        }),
                    totalDurationMs = report.TotalDuration.TotalMilliseconds
                };
                await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        });

        return app;
    }
}