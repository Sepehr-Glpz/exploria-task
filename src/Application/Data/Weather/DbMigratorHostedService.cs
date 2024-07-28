
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SGSX.Exploria.Application.Data.Weather;
internal class DbMigratorHostedService(IServiceProvider serviceProvider, ILogger<DbMigratorHostedService> logger) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<DbMigratorHostedService> _logger = logger;
    public async Task StartAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var ctx = scope.ServiceProvider.GetRequiredService<WeatherDatabaseContext>();

            await ctx.Database.MigrateAsync(ct);
        }
        catch (Exception ex)
        {
            _logger
                .LogCritical(ex, "failed to migrate database!");
        }
    }

    public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}
