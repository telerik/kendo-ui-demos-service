using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace kendo_northwind_pg
{
    public static class SessionUtils
    {
        public static string GetUserKey(IHttpContextAccessor _contextAccessor)
        {
            var identifierBuilder = new StringBuilder();
            var connectionID = _contextAccessor.HttpContext.Connection.Id.Substring(0, 11);

            identifierBuilder.Append(connectionID);

            var ipAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            identifierBuilder.Append(ipAddress);

            var userAgent = _contextAccessor.HttpContext.Response.Headers["User-Agent"].ToString();
            identifierBuilder.Append(userAgent);

            var identifier = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(identifierBuilder.ToString()))).Substring(0, 32);

            return identifier;
        }
    }
}
