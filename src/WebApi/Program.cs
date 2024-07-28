using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SGSX.Exploria.Application;

namespace WebApi;
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            #region Building the app

            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllers();

            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            builder.Services.AddApplicationLayer(builder.Configuration);

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("weather", new OpenApiInfo()
                {
                    Title = "Exploria Task",
                    Description = "simple weather wrapper api for exploria",
                    Version = "1.0",
                });
            });

            // tried this, it does not work right

            //builder.Services.AddRequestTimeouts(c =>
            //{
            //    c.DefaultPolicy = new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy()
            //    {
            //        Timeout = TimeSpan.FromSeconds(5),
            //    };
            //});

            #endregion

            #region Configure pipeline

            using var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(ui =>
                {
                    ui.SwaggerEndpoint("/swagger/weather/swagger.json", "weather");
                });
            }

            app.UseRouting();

            // dont use this
            //app.UseRequestTimeouts();

            app.MapControllers();

            #endregion

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application stopped. reason: {ex.Message}");
        }
    }
}
