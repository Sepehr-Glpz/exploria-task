
namespace SGSX.Exploria.Application.Models;
public class WeatherReport
{
    public required Guid Id { get; init; }
    public required byte[] Data { get; init; }
    public DateTime ReportDate { get; init; } = DateTime.Now;
}
