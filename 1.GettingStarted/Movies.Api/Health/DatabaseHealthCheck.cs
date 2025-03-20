using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.Api.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    private const string errorMsg = "Database is unhealthy";

    public const string Name = "Database";

    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            _ = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            _logger.LogError(errorMsg, ex);
            return HealthCheckResult.Unhealthy(errorMsg, ex);
        }
    }
}
