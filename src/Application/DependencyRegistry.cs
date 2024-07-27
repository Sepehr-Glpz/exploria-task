using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using SGSX.Exploria.Application.Data;
using SGSX.Exploria.Application.Infra;
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
            });

        services.AddScoped<WeatherService>();
        services.AddScoped<WeatherProviderService>();
        services.AddDbContext<WeatherDatabaseContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"), sql =>
            {
                sql.EnableRetryOnFailure(3);
            });
        });

        services.AddSingleton<NewReportNotifier>();

        services.AddHostedService<DbMigratorHostedService>();
        services.AddHostedService<ReportPersisterBackgroundService>();
    }
}
