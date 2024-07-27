using Microsoft.EntityFrameworkCore;
using SGSX.Exploria.Application.Models;

namespace SGSX.Exploria.Application.Data;
public sealed class WeatherDatabaseContext(DbContextOptions<WeatherDatabaseContext> options) : DbContext(options)
{
    public DbSet<WeatherReport> Reports { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .Entity<WeatherReport>()
            .ToTable(nameof(WeatherReport))
            .HasKey(k => k.Id);
        builder.Entity<WeatherReport>()
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .IsRequired()
            .HasColumnType("UNIQUEIDENTIFIER");
        builder.Entity<WeatherReport>()
            .Property(c => c.ReportDate)
            .IsRequired()
            .HasColumnType("DATETIME2(3)");
        builder.Entity<WeatherReport>()
            .Property(c => c.Data)
            .IsRequired()
            .HasColumnType("VARBINARY(MAX)");

        builder.Entity<WeatherReport>()
            .HasIndex(c => c.ReportDate)
            .HasDatabaseName($"IX_Dbo_WeatherReport({nameof(WeatherReport.ReportDate)})");

    }
}
