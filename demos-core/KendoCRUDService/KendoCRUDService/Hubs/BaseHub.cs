using Microsoft.AspNetCore.SignalR;

namespace KendoCRUDService.Hubs
{
    public class BaseHub : Hub
    {
        protected string GetGroupName()
        {
            return GetRemoteIpAddress();
        }

        protected string GetRemoteIpAddress()
        {
            return Context.GetHttpContext()?.Connection.RemoteIpAddress.ToString();
        }
    }
}
