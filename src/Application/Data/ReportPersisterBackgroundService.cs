using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGSX.Exploria.Application.Infra;

namespace SGSX.Exploria.Application.Data;
internal class ReportPersisterBackgroundService(
    NewReportNotifier newReportNotifier,
    IDbContextFactory<WeatherDatabaseContext> ctxFactory,
    ILogger<ReportPersisterBackgroundService> logger) : BackgroundService
{
    private readonly NewReportNotifier _reportNotifier = newReportNotifier;
    private readonly IDbContextFactory<WeatherDatabaseContext> _ctxFactory = ctxFactory;
    private readonly ILogger<ReportPersisterBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach(var report in _reportNotifier.ListenAsync())
        {
            try
            {
                using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);

                await ctx.Reports
                    .Upsert(report)
                    .On(c => c.Id)
                    .UpdateIf((prev, current) => current.ReportDate > prev.ReportDate)
                    .RunAsync(stoppingToken);
            }
            catch(Exception ex)
            {
                _logger
                    .LogWarning(ex, "failed to persist new report");

                continue;
            }
        }
    }
}
