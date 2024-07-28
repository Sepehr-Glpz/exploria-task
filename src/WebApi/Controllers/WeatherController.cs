using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGSX.Exploria.Application.Models.Weather;
using SGSX.Exploria.Application.Services;
using System.Net.Mime;

namespace SGSX.Exploria.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json, MediaTypeNames.Text.Plain)]
[Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Text.Plain)]
public class WeatherController(WeatherService weatherService) : ControllerBase
{
    private readonly WeatherService _weatherService = weatherService;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get([FromQuery] Dictionary<string, string> query)
    {
        var cancellationTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(4));

        var response = await _weatherService.GetWeatherReportAsync(new WeatherReportQuery()
        {
            Params = query,
        }, cancellationTimeout.Token);

        if(response.IsSuccess)
        {
            return File(response.Value.Data, MediaTypeNames.Application.Json);
        }
        else
        {
            return NoContent();
        }
    }
}
