﻿using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGSX.Exploria.Application.Data.Weather;
using SGSX.Exploria.Application.Infra;
using SGSX.Exploria.Application.Models.Weather;

namespace SGSX.Exploria.Application.Services;
public class WeatherService(WeatherDatabaseContext databaseContext,
    WeatherProviderService weatherProvider,
    NewReportNotifier reportNotifier,
    ILogger<WeatherService> logger)
{
    #region Deps

    private readonly ILogger<WeatherService> _logger = logger;
    private readonly WeatherDatabaseContext _dbContext = databaseContext;
    private readonly WeatherProviderService _weatherProviderService = weatherProvider;
    private readonly NewReportNotifier _newReportNotifier = reportNotifier;

    #endregion

    #region Methods

    public async Task<Result<WeatherReport>> GetWeatherReportAsync(WeatherReportQuery query, CancellationToken ct)
    {
        try
        {
            var reportId = query.ToUniqueId();

            if (reportId == Guid.Empty)
                return Result.Fail("invalid query!");

            var dbQueryCancel = GetDependentCancelSrc(ct); // a token thats cancelled if main request is cancelled

            var prevReportTask = GetReportById(reportId, dbQueryCancel.Token); // run the db call in parallel

            var currentWeather = await GetReportFromProvider(query, ct); // wait for the main result

            if(currentWeather.IsSuccess)
            {
                var result = new WeatherReport()
                {
                    Id = reportId,
                    Data = currentWeather.Value,
                    ReportDate = DateTime.Now,
                };

                await _newReportNotifier.Notify(result);

                dbQueryCancel.Cancel();

                return Result.Ok(result);
            }

            if (!prevReportTask.IsCompleted && !ct.IsCancellationRequested)
                await prevReportTask;

            return prevReportTask.Result switch
            {
                { IsSuccess: true } => prevReportTask.Result,
                _ => Result.Fail("failed to create report")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "failed to create report!");

            return Result.Fail(ex.Message);
        }
    }

    #endregion

    #region Private Util

    private async Task<Result<byte[]>> GetReportFromProvider(WeatherReportQuery query, CancellationToken ct)
    {
        try
        {
            return await _weatherProviderService.GetWeatherAsync(query.Params, ct);
        }
        catch(OperationCanceledException)
        {
            return Result.Fail("provider call cancelled");
        }
    }

    private async Task<Result<WeatherReport>> GetReportById(Guid id, CancellationToken ct)
    {
        try
        {
            var prevReport = await _dbContext
                .Reports
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync(ct);

            if (prevReport is null)
            {
                return Result.Fail("report not found!");
            }

            return Result.Ok(prevReport);
        }
        catch(Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    private static CancellationTokenSource GetDependentCancelSrc(CancellationToken ct)
    {
        var cancelSrc = new CancellationTokenSource();
        ct.Register(() => cancelSrc.Cancel());

        return cancelSrc;
    }

    #endregion
}
