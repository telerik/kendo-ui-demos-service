using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace KendoCoreService.Filters
{
    public class RestrictDomainAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedDomains;

        public RestrictDomainAttribute(params string[] allowedDomains)
        {
            _allowedDomains = allowedDomains;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var origin = context.HttpContext.Request.Headers["Origin"].ToString();
            var referer = context.HttpContext.Request.Headers["Referer"].ToString();

            bool isAllowed = !string.IsNullOrEmpty(origin) && _allowedDomains.Any(d => origin.Contains(d)) ||
                             !string.IsNullOrEmpty(referer) && _allowedDomains.Any(d => referer.Contains(d));

            if (!isAllowed)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "Access denied. Your domain is not allowed."
                };
            }

            base.OnActionExecuting(context);
        }
    }
}
