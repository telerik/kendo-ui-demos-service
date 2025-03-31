using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace KendoCRUDService
{
    public static class SessionUtils
    {
        public static string GetUserKey(IHttpContextAccessor _contextAccessor)
        {
            var identifierBuilder = new StringBuilder();

            var ipAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            identifierBuilder.Append(ipAddress);

            var userAgent = _contextAccessor.HttpContext.Response.Headers["User-Agent"].ToString();
            identifierBuilder.Append(userAgent);

            var identifier = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(identifierBuilder.ToString()))).Substring(0, 32);

            return identifier;
        }
    }
}
