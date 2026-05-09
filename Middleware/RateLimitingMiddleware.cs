using System.Collections.Concurrent;
using System.Net;

namespace CafeManagement.Middleware
{
    // 🛡️ Phase 5.1: Custom Rate Limiting to prevent bot attacks
    // 🛠️ RELAXED for development to avoid blocking the user
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, List<DateTime>> _requestTimes = new();
        private const int RequestLimit = 10000; // Increased significantly for dev
        private const int TimeWindowSeconds = 60;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            var timestamps = _requestTimes.GetOrAdd(ipAddress, _ => new List<DateTime>());

            lock (timestamps)
            {
                timestamps.RemoveAll(t => t < now.AddSeconds(-TimeWindowSeconds));
                if (timestamps.Count >= RequestLimit)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "text/plain";
                    context.Response.WriteAsync("🚀 Too many requests! Please wait a minute.").Wait();
                    return;
                }
                timestamps.Add(now);
            }

            await _next(context);
        }
    }

    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}
