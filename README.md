# API Aggregation Service

A .NET-based aggregation service that merges data from multiple external APIs (NewsAPI, RAWG, OpenWeather), applies caching, resilience policies, JWT authentication, collects performance statistics, and runs background anomaly detection.

---

## 📂 Repository Layout

/ (solution root)
├── ApiAggregator/ # main ASP.NET Core project
├── Tests/ # xUnit test project
└── README.md # this file

"Clients": {
  "NewsApi":     { "BaseUrl": "https://newsapi.org/v2",    "ApiKey": "..." },
  "Rawg":        { "BaseUrl": "https://api.rawg.io/api",   "ApiKey": "..." },
  "OpenWeather": { "BaseUrl": "https://api.openweathermap.org/data/2.5/weather", "ApiKey": "...", "Units": "metric" }
},
"Jwt": {
  "Key":      "<32-byte-base64-secret>",
  "Issuer":   "https://your.issuer",
  "Audience": "your-api"
}


🔒 Security

    Obtain a JWT token:

curl -X POST https://localhost:5001/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"<user>","password":"<pass>"}'


Response:

{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." }

Use the token for protected endpoints:

Authorization: Bearer <token>


🚀 Endpoints
Authentication

    POST /api/auth/login

        Body: { "username":"...", "password":"..." }

        Returns: { "token":"<jwt>" }

Aggregation

    GET /api/aggregate

        Query parameters:

            city (required)

            query (required)

            page, pageSize, from, to, sort (optional)

        Header: Authorization: Bearer <jwt>

        Returns aggregated JSON from NewsAPI, RAWG, and OpenWeather

Statistics

    GET /api/statistics

        Returns per-API: total requests, average latency, performance buckets     



🛠️ Caching & Resilience

    IMemoryCache with TTL per client:

        NewsAPI: 1 minute

        RAWG: 10 minutes

        OpenWeather: 10 minutes

    Polly policies on each HttpClient:

        Retry (3× with exponential back-off)

        CircuitBreaker (5 failures → open 1 minute)


📊 Background Anomaly Detection

    PerformanceMonitor (hosted service):

        Runs every 1 minute

        Compares 5 min sliding-window average vs. global average

        Logs a warning if window avg > 150 % of global avg        


✅ Unit Tests

    Located in ApiAggregator.Tests/

    Built with xUnit & Moq

    Covers:

        AggregationRequest validation     

        
