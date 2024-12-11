using System.Collections.Concurrent;

    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimits = new();
        private static readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1); // ¬ремеви прозорец
        private static readonly int _requestLimit = 100; // ћаксимален брой за€вки на потребител за времеви прозорец

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress))
            {
                await _next(context);
                return;
            }

            var rateLimitInfo = _rateLimits.GetOrAdd(ipAddress, new RateLimitInfo { LastRequestTime = DateTime.UtcNow });

            if (DateTime.UtcNow - rateLimitInfo.LastRequestTime > _timeWindow)
            {
                rateLimitInfo.RequestCount = 0;
                rateLimitInfo.LastRequestTime = DateTime.UtcNow;
            }

            rateLimitInfo.RequestCount++;

            if (rateLimitInfo.RequestCount > _requestLimit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                return;
            }

            await _next(context);
        }

        private class RateLimitInfo
        {
            public int RequestCount { get; set; }
            public DateTime LastRequestTime { get; set; }
        }
    }
