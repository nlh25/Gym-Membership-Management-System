using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using GMMS.Database.AppDbContextModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GMMS.Api.Health
{
    // Health check that uses AppDbContext to verify DB connectivity
    public class DbHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider _provider;

        public DbHealthCheck(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var canConnect = await db.Database.CanConnectAsync(cancellationToken);
                return canConnect ? HealthCheckResult.Healthy("Database reachable") : HealthCheckResult.Unhealthy("Cannot connect to database");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message, ex);
            }
        }
    }
}
