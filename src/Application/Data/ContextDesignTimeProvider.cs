
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SGSX.Exploria.Application.Data;
internal class ContextDesignTimeProvider : IDesignTimeDbContextFactory<WeatherDatabaseContext>
{
    private const string LOCAL_DB_CONN = "Server=127.0.0.1;User Id=application;Password=123456;Database=WeatherTask;Encrypt=False";

    private readonly DbContextOptions<WeatherDatabaseContext> _options =
        new DbContextOptionsBuilder<WeatherDatabaseContext>()
        .UseSqlServer(LOCAL_DB_CONN)
        .Options;

    public WeatherDatabaseContext CreateDbContext(string[] args)
    {
        return new WeatherDatabaseContext(_options);
    }
}
