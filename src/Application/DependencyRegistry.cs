using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using SGSX.Exploria.Application.Data.Weather;
using SGSX.Exploria.Application.Infra;
using SGSX.Exploria.Application.Models.Configuration;
using SGSX.Exploria.Application.Services;
using System.Net.Http;

namespace SGSX.Exploria.Application;
public static class DependencyRegistry
{
    public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpClient<HttpClient>()
            .AddStandardResilienceHandler(res =>
            {
                res.CircuitBreaker.MinimumThroughput = 500;
                res.Retry.ShouldHandle = (_) => ValueTask.FromResult(false);
            });

        services.AddDbContext<WeatherDatabaseContext>((sp, options) =>
        {
            var config = sp.CreateScope().ServiceProvider.GetRequiredService<IOptionsSnapshot<DatabaseConnectionConfig>>();
            options.UseSqlServer(config.Value.Default);
        });

        services.AddScoped<WeatherService>();
        services.AddScoped<WeatherProviderService>();
        services.AddSingleton<NewReportNotifier>();
        services.AddScoped<ConfigurationService>();
        services.Configure<WeatherApiConfig>(configuration.GetSection(nameof(WeatherApiConfig)));
        services.Configure<DatabaseConnectionConfig>(configuration.GetSection("ConnectionStrings"));

        services.AddHostedService<DbMigratorHostedService>();
        services.AddHostedService<ReportPersisterBackgroundService>();
    }
}
