using Microsoft.AspNetCore.Builder;

namespace WebApi;
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            using var app = builder.Build();

            await app.RunAsync();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Application stopped. reason: {ex.Message}");
        }
    }
}
