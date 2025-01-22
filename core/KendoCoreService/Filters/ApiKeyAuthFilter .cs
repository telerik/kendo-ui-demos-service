using KendoCoreService.Settings;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KendoCoreService.Filters
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly ApiKeySettings _apiKeySettings;

        public ApiKeyAuthFilter(IOptions<ApiKeySettings> apiKeySettings)
        {
            _apiKeySettings = apiKeySettings.Value;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!_apiKeySettings.Keys.Values.Any(x => x == extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
