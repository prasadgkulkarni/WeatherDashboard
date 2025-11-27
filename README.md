# Weather Dashboard

## Project Structure
- `backend/WeatherDashboard.Api/` – ASP.NET Core API.
- `backend/WeatherDashboard.Tests/` – xUnit unit tests for the API.

## Prerequisites
- OpenWeatherMap API key.

## Back-end
Configuration (can be set via environment variables or `appsettings.json`):
- `OpenWeather__ApiKey` – required for live weather calls.

### API Endpoints
- `GET /api/weather?city={city}` – current weather for a city.
- `GET /api/settings/default-location` – returns `{ city }` if set, else 404.
- `PUT /api/settings/default-location` – body `{ "city": "London" }`, sets default.

### Back-end Tests
cd backend/WeatherDashboard.Tests
```

## Notes on Architecture
- **Back-end**: Minimal hosting model; `OpenWeatherService` encapsulates provider access and caches responses for 5 minutes via `IMemoryCache`; `DefaultLocationStore` is in-memory but easily replaceable with persistent storage; controllers handle validation, logging, and friendly errors.
- **Testing**:  API logic covered with xUnit + Moq-style doubles for HTTP and storage.