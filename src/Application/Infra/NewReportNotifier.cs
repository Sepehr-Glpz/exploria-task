﻿using SGSX.Exploria.Application.Models.Weather;
using System.Threading.Channels;

namespace SGSX.Exploria.Application.Infra;
public class NewReportNotifier
{
    private readonly Channel<WeatherReport> _channel = Channel.CreateUnbounded<WeatherReport>();

    public async ValueTask Notify(WeatherReport report) => await _channel.Writer.WriteAsync(report);

    public IAsyncEnumerable<WeatherReport> ListenAsync(CancellationToken ct) => _channel.Reader.ReadAllAsync(ct);
}
