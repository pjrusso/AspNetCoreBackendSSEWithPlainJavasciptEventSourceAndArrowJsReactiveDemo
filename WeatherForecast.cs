using System.Text.Json.Serialization;

namespace AspNetCoreWebApiWithSSE
{
    public class WeatherForecast
    {
        // We keep an Id for server-side tracking and as the SSE event id,
        // but we don't want to include it in the JSON payload sent to clients.
        [JsonIgnore]
        public Guid Id { get; set;  } = default(Guid);

        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}
