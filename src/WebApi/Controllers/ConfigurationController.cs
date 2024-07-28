using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGSX.Exploria.Application.Models.Configuration;
using SGSX.Exploria.Application.Services;
using SGSX.Exploria.WebApi.ViewModels;
using System.Net.Mime;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace SGSX.Exploria.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Text.Plain)]
[Produces(MediaTypeNames.Application.Json, MediaTypeNames.Text.Plain)]
public class ConfigurationController(
    ConfigurationService configurationService,
    IValidator<DatabaseConfigViewModel> dbValidator,
    IValidator<WeatherApiConfigViewModel> apiValidator) : ControllerBase()
{
    private readonly IValidator<DatabaseConfigViewModel> _dbConfigValidator = dbValidator;
    private readonly IValidator<WeatherApiConfigViewModel> _apiConfigValidator = apiValidator;
    private readonly ConfigurationService _configurationService = configurationService;

    [HttpPut("database")]
    public IActionResult UpdateDatabase([FromBody] DatabaseConfigViewModel configViewModel)
    {
        var validation = _dbConfigValidator.Validate(configViewModel);

        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(s => s.ErrorMessage) });

        var response = _configurationService.UpdateDatabaseConfig(new DatabaseConnectionConfig()
        {
            Default = configViewModel.DefaultConnectionString,
        });

        if (response.IsSuccess)
            return Ok();
        else
            return Problem(response.Errors[0].Message, statusCode: StatusCodes.Status500InternalServerError);

    }

    [HttpPut("weather")]
    public IActionResult UpdateWeatherApi([FromBody] WeatherApiConfigViewModel configViewModel)
    {
        var validation = _apiConfigValidator.Validate(configViewModel);

        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(s => s.ErrorMessage) });

        var response = _configurationService.UpdateApiConfig(new WeatherApiConfig()
        {
            WeatherApiBaseUrl = configViewModel.WeatherApiBaseUrl,
            WeatherApiPath = configViewModel.WeatherApiPath,
        });

        if (response.IsSuccess)
            return Ok();
        else
            return Problem(response.Errors[0].Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}
