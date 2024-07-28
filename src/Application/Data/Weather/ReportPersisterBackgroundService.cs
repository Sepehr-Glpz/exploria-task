using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGSX.Exploria.Application.Infra;

namespace SGSX.Exploria.Application.Data.Weather;
internal class ReportPersisterBackgroundService(
    NewReportNotifier newReportNotifier,
    IServiceProvider sp,
    ILogger<ReportPersisterBackgroundService> logger) : BackgroundService
{
    #region Deps

    private readonly NewReportNotifier _reportNotifier = newReportNotifier;
    private readonly IServiceProvider _serviceProvider = sp;
    private readonly ILogger<ReportPersisterBackgroundService> _logger = logger;

    #endregion

    #region Methods

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var report in _reportNotifier.ListenAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
               var ctx = scope.ServiceProvider.GetRequiredService<WeatherDatabaseContext>();

                await ctx.Reports
                    .Upsert(report)
                    .On(c => c.Id)
                    .UpdateIf((prev, current) => current.ReportDate > prev.ReportDate)
                    .RunAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger
                    .LogWarning(ex, "failed to persist new report");

                continue;
            }
        }
    }

    #endregion
}
