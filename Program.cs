
using System.Text.Json;

namespace AspNetCoreWebApiWithSSE
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Add a permissive CORS policy for the demo (allow anything)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // Enable the demo CORS policy
            app.UseCors("AllowAll");

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");

            app.MapGet("/weatherforecastevents", async (HttpContext context) =>
            {
                context.Response.Headers.ContentType = "text/event-stream";
                context.Response.Headers.CacheControl = "no-cache";
                context.Response.Headers.Connection = "keep-alive";


                var forecast = Enumerable.Range(1, 1000).Select(index =>
                    new WeatherForecast
                    {
                        Id = Guid.NewGuid(),
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();

                for (var i = 0; i < 1000; i++)
                {
                    var json = JsonSerializer.Serialize(forecast[i]);

                    // Use the forecast Id as the SSE event id so clients can track messages
                    await context.Response.WriteAsync($"id: {forecast[i].Id}\n");

                    // Use the forecast summary as the SSE event type so clients can filter by event type if they want
                    await context.Response.WriteAsync($"event: weather\n");

                    // Finally write the forecast data as JSON in the SSE data field...
                    await context.Response.WriteAsync($"data: {json}\n\n");

                    await context.Response.Body.FlushAsync();

                    await Task.Delay(Random.Shared.Next(1000, 5000));
                }
            })
            .WithName("GetWeatherForecastEvents");

            app.MapGet("/api/events", async context =>
            {
                context.Response.Headers.ContentType = "text/event-stream";

                for (var i = 1; i <= 10; i++)
                {
                    await context.Response.WriteAsync(
                        $"data: {{\"text\":\"Message {i}\"}}\n\n"
                    );

                    await context.Response.Body.FlushAsync();

                    await Task.Delay(1000);
                }
            })
            .WithName("events");

            await app.RunAsync();
        }
    }
}
