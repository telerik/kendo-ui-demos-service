using System.Text;

public class ODataBatchBodyPreProcessMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly ILogger<ODataBatchBodyPreProcessMiddleware> _logger;

    public ODataBatchBodyPreProcessMiddleware(RequestDelegate next, IHttpContextAccessor httpAccessor, ILogger<ODataBatchBodyPreProcessMiddleware> logger)
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
            var scheme = context.Request.Scheme; // e.g., "https"
            var host = context.Request.Host.Value; // e.g., "mydomain.com" or "localhost:5001"
            var fullUrl = $"{scheme}://{host}/service/v2/odata";
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var requestBody = await reader.ReadToEndAsync();
                var modifiedBody = requestBody.Replace(fullUrl, String.Empty);
                _logger.LogInformation($"Modified Request Body:\n{modifiedBody}");
                var bytes = Encoding.UTF8.GetBytes(modifiedBody);
                var newBody = new MemoryStream(bytes);
                context.Request.Body = newBody;
                context.Request.Body.Position = 0;
                context.Request.ContentLength = bytes.Length;
            }

            await _next(context);
        }
        else
        {
            await _next(context);
        }
    }
}