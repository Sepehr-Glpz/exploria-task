
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SGSX.Exploria.Application.Data;
internal class DbMigratorHostedService(IServiceProvider serviceProvider) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();

        var ctx = scope.ServiceProvider.GetRequiredService<WeatherDatabaseContext>();

        await ctx.Database.MigrateAsync(ct);
    }

    public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}
