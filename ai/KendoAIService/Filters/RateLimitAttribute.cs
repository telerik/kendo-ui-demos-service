using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;

namespace KendoAIService.Filters
{
    public class RateLimitAttribute : ActionFilterAttribute
    {
        private static readonly ConcurrentDictionary<string, RequestRateLimiter> _requestLimits = new();
        private readonly int _maxRequests;
        private readonly int _timeWindowInSeconds;

        public RateLimitAttribute(int maxRequests, int timeWindowInSeconds)
        {
            _maxRequests = maxRequests;
            _timeWindowInSeconds = timeWindowInSeconds;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var limiter = _requestLimits.GetOrAdd(ipAddress, new RequestRateLimiter(_maxRequests, _timeWindowInSeconds));
            if (!limiter.IsRequestAllowed())
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status429TooManyRequests,
                    Content = "Too many requests. Please try again later."
                };
            }
        }
    }
}
