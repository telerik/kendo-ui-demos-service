using System.Text;

public class ODataBatchHttpContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly ILogger<ODataBatchHttpContextMiddleware> _logger;

    public ODataBatchHttpContextMiddleware(RequestDelegate next, IHttpContextAccessor httpAccessor, ILogger<ODataBatchHttpContextMiddleware> logger)
    {
        _next = next;
        _httpAccessor = httpAccessor;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _httpAccessor.HttpContext ??= context;

        if (context.Request.Path.Value != null && context.Request.Path.Value != "/health")
        {
            _logger.LogInformation($"Method: {context.Request.Method}");
            _logger.LogInformation($"Path: {context.Request.Path}");
            _logger.LogInformation($"Query: {context.Request.QueryString}");
            
            foreach (var header in context.Request.Headers)
            {
                _logger.LogInformation($"Header: {header.Key} = {header.Value}");
            }

            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var requestBody = await reader.ReadToEndAsync();
                _logger.LogInformation($"Request Body:\n{requestBody}");
                context.Request.Body.Position = 0;
            }

            await _next(context);
        }
        else
        {
            await _next(context);
        }
    }
}