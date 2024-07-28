using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGSX.Exploria.Application.Models.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;

namespace SGSX.Exploria.Application.Infra;
public class WeatherProviderService(
    IOptionsSnapshot<WeatherApiConfig> config,
    IHttpClientFactory httpClientFactory,
    ILogger<WeatherProviderService> logger)
{
    #region Deps

    private readonly IOptionsSnapshot<WeatherApiConfig> _config = config;
    private readonly IHttpClientFactory _clientFactory = httpClientFactory;
    private readonly ILogger<WeatherProviderService> _logger = logger;

    #endregion 

    #region Methods

    public async Task<Result<byte[]>> GetWeatherAsync(IReadOnlyDictionary<string, string> query, CancellationToken ct)
    {
        try
        {
            using var client = _clientFactory.CreateClient();

            var url = CreateRequestUri(query);

            var response = await client.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
                return Result.Fail("api response did not signify success");

            var result = await response.Content.ReadAsByteArrayAsync(ct);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger
                .LogError(ex, "failed to call the api");

            return Result.Fail(ex.Message);
        }
    }

    #endregion

    #region Private Util

    private Uri CreateRequestUri(IReadOnlyDictionary<string, string> query)
    {
        var builder = new UriBuilder(_config.Value.WeatherApiBaseUrl)
        {
            Path = _config.Value.WeatherApiPath,
            Query = CreateQueryString(query)
        };

        return builder.Uri;
    }

    private static string CreateQueryString(IReadOnlyDictionary<string, string> query) =>
        string.Join('&',
            query.Select(s => $"{WebUtility.UrlEncode(s.Key)}={WebUtility.UrlEncode(s.Value)}")
        );

    #endregion
}
