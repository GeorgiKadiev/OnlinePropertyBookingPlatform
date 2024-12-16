using System.Collections.Concurrent;

    public class RateLimitingMiddleware
    {
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimits = new();
    private readonly int _requestLimit;
    private readonly TimeSpan _timeWindow;
    private static readonly SemaphoreSlim _semaphore = new(1, 1); // Асинхронен семафор


    public RateLimitingMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        // Четене на конфигурация от appsettings.json
        _requestLimit = configuration.GetValue<int>("RateLimiting:RequestLimit", 100); // По подразбиране: 100 заявки
        _timeWindow = TimeSpan.FromSeconds(configuration.GetValue<int>("RateLimiting:TimeWindowInSeconds", 60)); // По подразбиране: 60 секунди
    }

    public async Task Invoke(HttpContext context)
        {
        // Получаване на IP и крайна точка
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        
            if (string.IsNullOrEmpty(ipAddress))
            {
              _logger.LogWarning("Could not determine IP address.");
               await _next(context);
               return;
            }

        var endpoint = context.Request.Path;
        var key = $"{ipAddress}:{endpoint}";


        await _semaphore.WaitAsync(); // Влизане в критична секция
        try
        {
            var rateLimitInfo = _rateLimits.GetOrAdd(key, k => new RateLimitInfo { LastRequestTime = DateTime.UtcNow });

            // Проверка дали времевият прозорец е изтекъл
            if (DateTime.UtcNow - rateLimitInfo.LastRequestTime > _timeWindow)
            {
                rateLimitInfo.RequestCount = 0;
                rateLimitInfo.LastRequestTime = DateTime.UtcNow;
            }

            rateLimitInfo.RequestCount++;

            // Проверка дали е достигнат лимитът
            if (rateLimitInfo.RequestCount > _requestLimit)
            {
                _logger.LogWarning($"Rate limit exceeded for IP: {ipAddress} on endpoint: {endpoint} at {DateTime.UtcNow}");
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Rate limit exceeded. Try again later.\"}");
                return;
            }
        }
        finally
        {
            _semaphore.Release(); // Излизане от критична секция
        }

        await _next(context);
    }
}

// Клас за съхранение на информация за лимита на заявки
    internal class RateLimitInfo
    {
        public int RequestCount { get; set; } // Трябва да бъде public, за да е достъпно
        public DateTime LastRequestTime { get; set; } // Трябва да бъде public, за да е достъпно
    }
   
