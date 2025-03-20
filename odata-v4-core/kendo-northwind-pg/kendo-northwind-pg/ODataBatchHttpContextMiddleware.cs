public class ODataBatchHttpContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpAccessor;

    public ODataBatchHttpContextMiddleware(RequestDelegate next, IHttpContextAccessor httpAccessor)
    {
        _next = next;
        _httpAccessor = httpAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _httpAccessor.HttpContext ??= context;

        await _next(context);
    }
}