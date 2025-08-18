using System;

namespace signalr_for_aspnet_core.Extensions
{
    public static class CorsExtension
    {
        public static bool IsOriginAllowed(string origin)
        {
            var uri = new Uri(origin);
            var allowedDomains = new string[] {
                "aspnet-core-demos-staging.azurewebsites.net",
                "aspnet-mvc-demos-staging.azurewebsites.net",
                "jquery-demos-staging.azurewebsites.net",
                "telerik.com",
                "demos.telerik.com",
                "127.0.0.1",
                "localhost",
                "dojo.telerik.com",
                "runner.telerik.io",
                "stackblitz.com",
                "codesandbox.io",
                "sitdemos.telerik.com"
            };

            var wildcardDomains = new string[] {
                "stackblitz.io",
                "telerik.com",
                "csb.app",
            };

            bool isAllowed = false;
            foreach (var allowedDomain in wildcardDomains)
            {
                if (uri.Host.EndsWith(allowedDomain, StringComparison.OrdinalIgnoreCase))
                {
                    isAllowed = true;
                    break;
                }
            }

            foreach (var allowedDomain in allowedDomains)
            {
                if (uri.Host.Equals(allowedDomain, StringComparison.OrdinalIgnoreCase))
                {
                    isAllowed = true;
                    break;
                }
            }

            return isAllowed;
        }
    }
}
