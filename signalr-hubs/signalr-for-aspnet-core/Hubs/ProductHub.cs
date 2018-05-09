using Microsoft.AspNetCore.SignalR;
using signalr_for_aspnet_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_for_aspnet_core.Hubs
{
    public class ProductHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName());
            return base.OnConnectedAsync();
        }

        public override System.Threading.Tasks.Task OnDisconnectedAsync(Exception e)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName());
            return base.OnDisconnectedAsync(e);
        }

        public IEnumerable<ProductSignalR> Read()
        {
            using (var context = new SampleEntitiesDataContext())
            {
                var createdAt = DateTime.Now;

                var products = context.Products
                                    .OrderBy(p => p.ProductName)
                                    .ToList()
                                    .Select((p, index) => new ProductSignalR
                                    {
                                        ID = (index + 1),
                                        ProductName = p.ProductName,
                                        UnitPrice = (double)p.UnitPrice.GetValueOrDefault(),
                                        UnitsInStock = p.UnitsInStock.GetValueOrDefault(),
                                        CreatedAt = createdAt = createdAt.AddMilliseconds(1)
                                    })
                                    .ToList();
                return products;
            }
        }

        public ProductSignalR Create(ProductSignalR product)
        {
            product.ID = DateTime.Now.Ticks;
            product.CreatedAt = DateTime.Now;

            Clients.OthersInGroup(GetGroupName()).SendAsync("create", product);

            return product;
        }

        public void Update(ProductSignalR product)
        {
            Clients.OthersInGroup(GetGroupName()).SendAsync("update", product);
        }

        public void Destroy(ProductSignalR product)
        {
            Clients.OthersInGroup(GetGroupName()).SendAsync("destroy", product);
        }

        public string GetGroupName()
        {
            return GetRemoteIpAddress();
        }

        public string GetRemoteIpAddress()
        {
            return Context.GetHttpContext()?.Connection.RemoteIpAddress.ToString();
        }
    }
}
