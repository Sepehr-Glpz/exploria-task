using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGSX.Exploria.Application.Models.Configuration;

namespace SGSX.Exploria.Application.Services;
public class ConfigurationService(ILogger<ConfigurationService> logger, IConfiguration configuration)
{
    private readonly ILogger<ConfigurationService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public Result UpdateDatabaseConfig(DatabaseConnectionConfig connectionConfig)
    {
        try
        {
            var section = _configuration.GetSection("ConnectionStrings");
            section[nameof(DatabaseConnectionConfig.Default)] = connectionConfig.Default;

            ReloadConfigAsRoot();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger
                .LogError(ex, "failed to update db config");

            return Result.Fail(ex.Message);
        }
    }

    public Result UpdateApiConfig(WeatherApiConfig apiConfig)
    {
        try
        {
            var section = _configuration.GetSection(nameof(WeatherApiConfig));
            section[nameof(WeatherApiConfig.WeatherApiBaseUrl)] = apiConfig.WeatherApiBaseUrl;
            section[nameof(WeatherApiConfig.WeatherApiPath)] = apiConfig.WeatherApiPath;

            ReloadConfigAsRoot();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger
                .LogError(ex, "failed to update db config");

            return Result.Fail(ex.Message);
        }
    }


    private void ReloadConfigAsRoot()
    {
        (_configuration as IConfigurationRoot)?.Reload();
    }
}
