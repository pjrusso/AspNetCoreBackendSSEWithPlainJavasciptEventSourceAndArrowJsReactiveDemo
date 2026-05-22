# ASP.NET Core SSE Demo with Plain JavaScript EventSource and Arrow.js Reactive

A small **ASP.NET Core Minimal API** demo for **Server-Sent Events (SSE)** with a plain browser `EventSource` client and an **Arrow.js reactive** HTML front-end page.

## Key Technologies

- **[.NET 10](https://dotnet.microsoft.com/)** – ASP.NET Core Minimal API (no controllers)
- **[Microsoft.AspNetCore.OpenApi](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview)** – OpenAPI/Swagger support in Development
- **[Server-Sent Events (SSE)](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events)** – one-way real-time push from server to browser over HTTP
- **[EventSource](https://developer.mozilla.org/en-US/docs/Web/API/EventSource)** – native browser API to consume SSE streams
- **[Arrow.js](https://www.arrow-js.com/)** (`@arrow-js/core`) – lightweight reactive UI library loaded from CDN; no build step required

## Repository Structure

```
├── Program.cs                          # App entry point, middleware setup, and all endpoints
├── WeatherForecast.cs                  # Data model (Date, TemperatureC, TemperatureF, Summary)
├── TestPageUsingArrowJsReactivity.html # Standalone browser client using Arrow.js reactivity
├── AspNetCoreWebApiWithSSE.csproj      # .NET 10 web project file
├── AspNetCoreWebApiWithSSE.slnx        # Solution file
├── app.http                            # Manual HTTP request samples for all endpoints
├── AspNetCoreWebApiWithSSE.http        # Additional HTTP request sample
├── appsettings.json                    # Logging and host configuration
├── appsettings.Development.json        # Development-specific overrides
└── Properties/
    └── launchSettings.json             # Dev launch URLs (https://localhost:7017, http://localhost:5068)
```

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/weatherforecast` | Returns a one-shot JSON array of 5 random weather forecasts |
| `GET` | `/weatherforecastevents` | SSE stream — emits 1,000 `weather` events with random delays (1–5 s each) |
| `GET` | `/api/events` | Simple SSE demo — emits 10 plain text messages, one per second |

### SSE event format (`/weatherforecastevents`)

```
event: weather
data: {"Date":"2026-05-23","TemperatureC":22,"TemperatureF":71,"Summary":"Warm"}
```

## How It Works

1. **Backend** sets `Content-Type: text/event-stream` and keeps the response connection open, flushing each event as it is generated.
2. **Frontend** (`TestPageUsingArrowJsReactivity.html`) opens a connection via the browser's `EventSource` API and listens for `weather` events.
3. **Arrow.js** wraps a plain JS object in `reactive({...})`, making the `connected` flag and `forecasts` array observable. Any mutation (e.g. `state.forecasts.push(forecast)`) automatically re-renders only the affected parts of the DOM.

## Running Locally

```bash
dotnet run --launch-profile https
```

Then open `TestPageUsingArrowJsReactivity.html` directly in a browser (double-click or `file://`). The page connects to `https://localhost:7017/weatherforecastevents` and streams weather events in real time, with a green/red LED indicator showing connection status.

> **Note:** Because the back end runs on `https://localhost:7017` with a dev certificate, you may need to trust the ASP.NET Core development certificate first:
> ```bash
> dotnet dev-certs https --trust
> ```
